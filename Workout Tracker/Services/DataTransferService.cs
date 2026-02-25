using System.Text.Json;
using ClosedXML.Excel;
using Workout_Tracker.Model;
using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.Services;

public class DataTransferService
{
    private const int CurrentVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<string> ExportAsync()
    {
        var db = AppDatabase.Database;

        var exportData = new ExportData
        {
            Version = CurrentVersion,
            ExportDate = DateTime.UtcNow,
            Data = new ExportPayload
            {
                Tags = await db.Table<Tag>().ToListAsync(),
                Muscles = await db.Table<Muscle>().ToListAsync(),
                Exercises = await db.Table<Exercise>().ToListAsync(),
                ExerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync(),
                Programs = await db.Table<Program>().ToListAsync(),
                Sessions = await db.Table<Session>().ToListAsync(),
                SessionExercises = await db.Table<SessionExercise>().ToListAsync(),
                Sets = await db.Table<Set>().ToListAsync(),
                BodyMetrics = await db.Table<BodyMetric>().ToListAsync(),
                RecoveryLogs = await db.Table<RecoveryLog>().ToListAsync(),
                CalorieLogs = await db.Table<CalorieLog>().ToListAsync(),
                SessionTags = await db.Table<SessionTag>().ToListAsync()
            }
        };

        var json = JsonSerializer.Serialize(exportData, JsonOptions);
        var fileName = $"workout_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(filePath, json);

        return filePath;
    }

    public (bool IsValid, string? ErrorMessage) ValidateImport(ExportData? data)
    {
        if (data == null)
            return (false, "This file is not a valid backup file.");

        if (data.Type is not null and not "full_backup")
            return (false, "This file is a program export, not a full backup. To import a program, use the Import option on the Programs page.");

        if (data.Version <= 0 || data.Version > CurrentVersion)
            return (false, "This backup was created by a newer version of the app. Please update the app first.");

        if (data.Data == null)
            return (false, "This backup file is empty or corrupted.");

        return (true, null);
    }

    // ── Program Export (XLSX) ──

    public async Task<string> ExportProgramAsync(int programId)
    {
        var db = AppDatabase.Database;

        var program = await db.Table<Program>().FirstOrDefaultAsync(p => p.Id == programId);
        if (program == null)
            throw new InvalidOperationException("Program not found.");

        var sessions = await db.Table<Session>()
            .Where(s => s.ProgramId == programId)
            .ToListAsync();

        var sessionIds = sessions.Select(s => s.Id).ToHashSet();

        var allSessionExercises = await db.Table<SessionExercise>().ToListAsync();
        var sessionExercises = allSessionExercises
            .Where(se => sessionIds.Contains(se.SessionId))
            .ToList();

        var seIds = sessionExercises.Select(se => se.Id).ToHashSet();

        var allSets = await db.Table<Set>().ToListAsync();
        var sets = allSets
            .Where(s => seIds.Contains(s.SessionExerciseId))
            .ToList();

        var exerciseIds = sessionExercises.Select(se => se.ExerciseId).Distinct().ToHashSet();
        var allExercises = await db.Table<Exercise>().ToListAsync();
        var exercises = allExercises.Where(e => exerciseIds.Contains(e.Id)).ToList();

        var allExerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
        var exerciseMuscles = allExerciseMuscles
            .Where(em => exerciseIds.Contains(em.ExerciseId))
            .ToList();

        var muscleIds = exerciseMuscles.Select(em => em.MuscleId).Distinct().ToHashSet();
        var allMuscles = await db.Table<Muscle>().ToListAsync();
        var muscles = allMuscles.Where(m => muscleIds.Contains(m.Id)).ToList();
        var muscleIdToName = muscles.ToDictionary(m => m.Id, m => m.Name);

        // Fetch session tags
        var allSessionTags = await db.Table<SessionTag>().ToListAsync();
        var tagsBySession = allSessionTags
            .Where(t => sessionIds.Contains(t.SessionId))
            .GroupBy(t => t.SessionId)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(t => t.TagName)));

        // Sort sessions by Week then Day, then by Date
        var orderedSessions = sessions
            .OrderBy(s => s.Week ?? int.MaxValue)
            .ThenBy(s => s.Day ?? int.MaxValue)
            .ThenBy(s => s.Date)
            .ToList();

        // Build lookup tables
        var exerciseMap = exercises.ToDictionary(e => e.Id);
        var seBySession = sessionExercises.GroupBy(se => se.SessionId)
            .ToDictionary(g => g.Key, g => g.OrderBy(se => se.Order).ToList());
        var setsBySe = sets.GroupBy(s => s.SessionExerciseId)
            .ToDictionary(g => g.Key, g => g.OrderBy(s => s.SetNumber).ToList());

        // Build muscle strings per exercise
        var exercisePrimaryMuscles = new Dictionary<int, string>();
        var exerciseSecondaryMuscles = new Dictionary<int, string>();
        foreach (var exId in exerciseIds)
        {
            var ems = exerciseMuscles.Where(em => em.ExerciseId == exId).ToList();
            var primary = ems.Where(em => em.Type == "primary")
                .Select(em => muscleIdToName.GetValueOrDefault(em.MuscleId, ""))
                .Where(n => !string.IsNullOrEmpty(n));
            var secondary = ems.Where(em => em.Type != "primary")
                .Select(em => muscleIdToName.GetValueOrDefault(em.MuscleId, ""))
                .Where(n => !string.IsNullOrEmpty(n));
            exercisePrimaryMuscles[exId] = string.Join(", ", primary);
            exerciseSecondaryMuscles[exId] = string.Join(", ", secondary);
        }

        // Parse program color
        XLColor programColor;
        try
        {
            programColor = XLColor.FromHtml(program.Color ?? "#4472C4");
        }
        catch
        {
            programColor = XLColor.FromHtml("#4472C4");
        }

        using var workbook = new XLWorkbook();

        // ── Sheet 1: Program ──
        BuildProgramSheet(workbook, program);

        // ── Sheet 2: Exercises ──
        BuildExercisesSheet(workbook, exercises, exercisePrimaryMuscles, exerciseSecondaryMuscles);

        // ── Sheet 3: Sessions (built after Workout to get row ranges) ──

        // ── Sheet 4: Workout ──
        var workoutSheet = workbook.Worksheets.Add("Workout");
        var wkHeaders = new[] { "Exercise", "Rest(s)", "Set", "W", "Plan", "PlannedWt", "Reps", "Weight", "Duration(s)", "RPE" };
        int wkRow = 1;
        var sessionWorkoutRowRanges = new List<(int StartRow, int EndRow)>();

        foreach (var session in orderedSessions)
        {
            // Session header row
            string headerText = session.Week.HasValue && session.Day.HasValue
                ? $"Week {session.Week} - Day {session.Day} | {session.Date:yyyy-MM-dd}"
                : $"Session | {session.Date:yyyy-MM-dd}";

            var headerRange = workoutSheet.Range(wkRow, 1, wkRow, 10);
            headerRange.Merge();
            workoutSheet.Cell(wkRow, 1).Value = headerText;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = programColor;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wkRow++;

            // Column headers
            for (int c = 0; c < wkHeaders.Length; c++)
            {
                workoutSheet.Cell(wkRow, c + 1).Value = wkHeaders[c];
                workoutSheet.Cell(wkRow, c + 1).Style.Font.Bold = true;
                workoutSheet.Cell(wkRow, c + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }
            wkRow++;

            int sessionDataStartRow = wkRow;

            // Exercise/set rows
            if (seBySession.TryGetValue(session.Id, out var sesExercises))
            {
                foreach (var se in sesExercises)
                {
                    if (!exerciseMap.TryGetValue(se.ExerciseId, out var exercise))
                        continue;

                    var exerciseSets = setsBySe.GetValueOrDefault(se.Id, []);
                    bool firstSet = true;

                    foreach (var set in exerciseSets)
                    {
                        if (firstSet)
                        {
                            workoutSheet.Cell(wkRow, 1).Value = exercise.Name;
                            workoutSheet.Cell(wkRow, 1).Style.Font.Bold = true;
                            if (se.RestSeconds.HasValue)
                                workoutSheet.Cell(wkRow, 2).Value = se.RestSeconds.Value;
                            firstSet = false;
                        }

                        workoutSheet.Cell(wkRow, 3).Value = set.SetNumber;
                        if (set.IsWarmup)
                            workoutSheet.Cell(wkRow, 4).Value = "W";

                        workoutSheet.Cell(wkRow, 5).Value = FormatPlan(set);
                        if (set.PlannedWeight.HasValue)
                            workoutSheet.Cell(wkRow, 6).Value = set.PlannedWeight.Value;

                        // Tracking columns (7-10) empty (clean template export)
                        wkRow++;
                    }

                    if (exerciseSets.Count == 0)
                    {
                        workoutSheet.Cell(wkRow, 1).Value = exercise.Name;
                        workoutSheet.Cell(wkRow, 1).Style.Font.Bold = true;
                        if (se.RestSeconds.HasValue)
                            workoutSheet.Cell(wkRow, 2).Value = se.RestSeconds.Value;
                        wkRow++;
                    }
                }
            }

            int sessionDataEndRow = Math.Max(wkRow - 1, sessionDataStartRow);
            sessionWorkoutRowRanges.Add((sessionDataStartRow, sessionDataEndRow));

            wkRow++; // empty row between sessions
        }

        workoutSheet.Columns().AdjustToContents();
        workoutSheet.Column(1).Width = Math.Max(workoutSheet.Column(1).Width, 25);

        // ── Sheet 3: Sessions (now that we know workout row ranges) ──
        BuildSessionsSheet(workbook, orderedSessions, sessionWorkoutRowRanges, tagsBySession);

        // Reorder sheets so Sessions comes before Workout
        workbook.Worksheet("Sessions").Position = 3;
        workbook.Worksheet("Workout").Position = 4;

        // Save
        var safeName = program.Name.Replace(" ", "_").Replace("/", "-");
        var fileName = $"program_{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await Task.Run(() => workbook.SaveAs(filePath));

        return filePath;
    }

    private static void BuildProgramSheet(XLWorkbook workbook, Program program)
    {
        var sheet = workbook.Worksheets.Add("Program");
        var meta = new (string Key, string Value)[]
        {
            ("Name", program.Name),
            ("Goal", program.Goal ?? ""),
            ("Start Date", program.StartDate.ToString("yyyy-MM-dd")),
            ("End Date", program.EndDate?.ToString("yyyy-MM-dd") ?? ""),
            ("Notes", program.Notes ?? ""),
            ("Color", program.Color ?? ""),
            ("Version", CurrentVersion.ToString()),
            ("Export Date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"))
        };

        sheet.Cell(1, 1).Value = "Key";
        sheet.Cell(1, 2).Value = "Value";
        sheet.Range(1, 1, 1, 2).Style.Font.Bold = true;

        for (int i = 0; i < meta.Length; i++)
        {
            sheet.Cell(i + 2, 1).Value = meta[i].Key;
            sheet.Cell(i + 2, 2).Value = meta[i].Value;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void BuildExercisesSheet(
        XLWorkbook workbook,
        List<Exercise> exercises,
        Dictionary<int, string> primaryMuscles,
        Dictionary<int, string> secondaryMuscles)
    {
        var sheet = workbook.Worksheets.Add("Exercises");
        var headers = new[] { "Id", "Name", "Equipment", "Type", "Time Based", "Description", "Instructions", "Notes", "Primary Muscles", "Secondary Muscles", "Media" };

        for (int c = 0; c < headers.Length; c++)
        {
            sheet.Cell(1, c + 1).Value = headers[c];
            sheet.Cell(1, c + 1).Style.Font.Bold = true;
        }

        int row = 2;
        foreach (var ex in exercises.OrderBy(e => e.Name))
        {
            sheet.Cell(row, 1).Value = ex.Id;
            sheet.Cell(row, 2).Value = ex.Name;
            sheet.Cell(row, 3).Value = ex.Equipment ?? "";
            sheet.Cell(row, 4).Value = ex.ExerciseType ?? "";
            sheet.Cell(row, 5).Value = ex.IsTimeBased ? "Yes" : "No";
            sheet.Cell(row, 6).Value = ex.Description ?? "";
            sheet.Cell(row, 7).Value = ex.Instructions ?? "";
            sheet.Cell(row, 8).Value = ex.Notes ?? "";
            sheet.Cell(row, 9).Value = primaryMuscles.GetValueOrDefault(ex.Id, "");
            sheet.Cell(row, 10).Value = secondaryMuscles.GetValueOrDefault(ex.Id, "");
            sheet.Cell(row, 11).Value = ex.ExampleMedia ?? "";
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void BuildSessionsSheet(
        XLWorkbook workbook,
        List<Session> orderedSessions,
        List<(int StartRow, int EndRow)> workoutRowRanges,
        Dictionary<int, string> tagsBySession)
    {
        var sheet = workbook.Worksheets.Add("Sessions");
        var headers = new[] { "Week", "Day", "Date", "Start Time", "End Time", "Energy", "Notes", "Completed", "Tags" };

        for (int c = 0; c < headers.Length; c++)
        {
            sheet.Cell(1, c + 1).Value = headers[c];
            sheet.Cell(1, c + 1).Style.Font.Bold = true;
        }

        for (int i = 0; i < orderedSessions.Count; i++)
        {
            var s = orderedSessions[i];
            int row = i + 2;

            if (s.Week.HasValue) sheet.Cell(row, 1).Value = s.Week.Value;
            if (s.Day.HasValue) sheet.Cell(row, 2).Value = s.Day.Value;
            sheet.Cell(row, 3).Value = s.Date.ToString("yyyy-MM-dd");
            // StartTime, EndTime, EnergyLevel stripped (clean template)
            sheet.Cell(row, 7).Value = s.Notes ?? "";

            // Completed formula: auto-calculate from Workout tracking data
            if (i < workoutRowRanges.Count)
            {
                var (startRow, endRow) = workoutRowRanges[i];
                sheet.Cell(row, 8).FormulaA1 =
                    $"IF(COUNTA(Workout!G{startRow}:I{endRow})>0,\"Yes\",\"No\")";
            }
            else
            {
                sheet.Cell(row, 8).Value = "No";
            }

            // Tags
            if (tagsBySession.TryGetValue(s.Id, out var tags))
                sheet.Cell(row, 9).Value = tags;
        }

        sheet.Columns().AdjustToContents();
    }

    private static string FormatPlan(Set set)
    {
        if (set.RepMin.HasValue || set.RepMax.HasValue)
        {
            if (set.RepMin.HasValue && set.RepMax.HasValue && set.RepMin != set.RepMax)
                return $"{set.RepMin}-{set.RepMax}";
            return (set.RepMin ?? set.RepMax)?.ToString() ?? "";
        }

        if (set.DurationMin.HasValue || set.DurationMax.HasValue)
        {
            if (set.DurationMin.HasValue && set.DurationMax.HasValue && set.DurationMin != set.DurationMax)
                return $"{set.DurationMin}-{set.DurationMax}s";
            return $"{set.DurationMin ?? set.DurationMax}s";
        }

        return "";
    }

    // ── Program Import (XLSX) ──

    public async Task<int> ImportProgramAsync(Stream stream)
    {
        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(stream);
        }
        catch
        {
            throw new InvalidOperationException("This file is not a valid XLSX program file.");
        }

        using (workbook)
        {
            if (!workbook.Worksheets.TryGetWorksheet("Program", out var programSheet))
                throw new InvalidOperationException("This XLSX file is missing the 'Program' sheet.");
            if (!workbook.Worksheets.TryGetWorksheet("Workout", out var workoutSheet))
                throw new InvalidOperationException("This XLSX file is missing the 'Workout' sheet.");

            workbook.Worksheets.TryGetWorksheet("Exercises", out var exercisesSheet);
            workbook.Worksheets.TryGetWorksheet("Sessions", out var sessionsSheet);

            // 1. Parse Program metadata
            var programMeta = ParseKeyValueSheet(programSheet);

            if (!programMeta.TryGetValue("Name", out var programName) || string.IsNullOrWhiteSpace(programName))
                throw new InvalidOperationException("Program name is missing from the XLSX file.");

            if (programMeta.TryGetValue("Version", out var versionStr) && int.TryParse(versionStr, out var version) && version > CurrentVersion)
                throw new InvalidOperationException("This program was exported by a newer version of the app. Please update the app first.");

            var newProgram = new Program
            {
                Name = programName,
                Goal = NullIfEmpty(programMeta.GetValueOrDefault("Goal")),
                StartDate = DateTime.TryParse(programMeta.GetValueOrDefault("Start Date"), out var sd) ? sd : DateTime.Today,
                EndDate = DateTime.TryParse(programMeta.GetValueOrDefault("End Date"), out var ed) ? ed : null,
                Notes = NullIfEmpty(programMeta.GetValueOrDefault("Notes")),
                Color = NullIfEmpty(programMeta.GetValueOrDefault("Color"))
            };

            // 2. Parse Exercises sheet
            var xlsxExercises = new List<XlsxExerciseData>();
            if (exercisesSheet != null)
                xlsxExercises = ParseExercisesSheet(exercisesSheet);

            // 3. Parse Sessions sheet
            var xlsxSessions = new List<XlsxSessionData>();
            if (sessionsSheet != null)
                xlsxSessions = ParseSessionsSheet(sessionsSheet);

            // 4. Parse Workout sheet → session blocks
            var workoutBlocks = ParseWorkoutSheet(workoutSheet);

            // ── Insert into database ──
            var db = AppDatabase.Database;

            // Resolve exercises: ID first, name fallback, create new if neither
            var existingMuscles = await db.Table<Muscle>().ToListAsync();
            var muscleNameToId = existingMuscles.ToDictionary(m => m.Name, m => m.Id);

            var existingExercises = await db.Table<Exercise>().ToListAsync();
            var existingExerciseById = existingExercises.ToDictionary(e => e.Id);
            var existingExerciseByName = existingExercises.ToDictionary(e => e.Name, e => e.Id);

            var exerciseNameToDbId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var xlsxExerciseByName = xlsxExercises.ToDictionary(e => e.Exercise.Name, e => e, StringComparer.OrdinalIgnoreCase);

            foreach (var xlsxEx in xlsxExercises)
            {
                var name = xlsxEx.Exercise.Name;

                // Try by ID first
                if (xlsxEx.Exercise.Id > 0 && existingExerciseById.ContainsKey(xlsxEx.Exercise.Id))
                {
                    exerciseNameToDbId[name] = xlsxEx.Exercise.Id;
                    continue;
                }

                // Try by name
                if (existingExerciseByName.TryGetValue(name, out var existingId))
                {
                    exerciseNameToDbId[name] = existingId;
                    continue;
                }

                // Create new exercise
                var newEx = new Exercise
                {
                    Name = xlsxEx.Exercise.Name,
                    Description = xlsxEx.Exercise.Description,
                    Instructions = xlsxEx.Exercise.Instructions,
                    Equipment = xlsxEx.Exercise.Equipment,
                    ExerciseType = xlsxEx.Exercise.ExerciseType,
                    ExampleMedia = xlsxEx.Exercise.ExampleMedia,
                    IsTimeBased = xlsxEx.Exercise.IsTimeBased,
                    Notes = xlsxEx.Exercise.Notes
                };
                await db.InsertAsync(newEx);
                exerciseNameToDbId[name] = newEx.Id;

                // Create muscle links for new exercise
                await CreateMuscleLinkAsync(db, newEx.Id, xlsxEx.PrimaryMuscles, "primary", muscleNameToId);
                await CreateMuscleLinkAsync(db, newEx.Id, xlsxEx.SecondaryMuscles, "secondary", muscleNameToId);
            }

            // Insert Program
            await db.InsertAsync(newProgram);

            // Insert Sessions and Workout data
            for (int i = 0; i < workoutBlocks.Count; i++)
            {
                var block = workoutBlocks[i];

                // Build Session from Sessions sheet (matched by order) or from workout header
                var session = new Session
                {
                    ProgramId = newProgram.Id,
                    Date = DateTime.Today
                };

                if (i < xlsxSessions.Count)
                {
                    var sd2 = xlsxSessions[i];
                    session.Week = sd2.Week;
                    session.Day = sd2.Day;
                    session.Date = sd2.Date;
                    session.StartTime = sd2.StartTime;
                    session.EndTime = sd2.EndTime;
                    session.EnergyLevel = sd2.EnergyLevel;
                    session.Notes = sd2.Notes;
                }
                else
                {
                    // Try parsing date from header
                    var datePart = block.HeaderText.Split('|').LastOrDefault()?.Trim();
                    if (DateTime.TryParse(datePart, out var headerDate))
                        session.Date = headerDate;

                    // Try parsing week/day from header
                    ParseHeaderWeekDay(block.HeaderText, out var week, out var day);
                    session.Week = week;
                    session.Day = day;
                }

                // Determine IsCompleted: true if any set has tracking data
                session.IsCompleted = block.Exercises
                    .SelectMany(e => e.Sets)
                    .Any(s => s.Reps.HasValue || s.Weight.HasValue || s.DurationSeconds.HasValue);

                await db.InsertAsync(session);

                // Insert exercises and sets for this session
                int order = 0;
                foreach (var exBlock in block.Exercises)
                {
                    // Resolve exercise ID
                    int exerciseDbId;
                    if (exerciseNameToDbId.TryGetValue(exBlock.ExerciseName, out var resolvedId))
                    {
                        exerciseDbId = resolvedId;
                    }
                    else if (existingExerciseByName.TryGetValue(exBlock.ExerciseName, out var nameMatchId))
                    {
                        exerciseDbId = nameMatchId;
                        exerciseNameToDbId[exBlock.ExerciseName] = nameMatchId;
                    }
                    else
                    {
                        // Exercise not in Exercises sheet — create with minimal data
                        var newEx = new Exercise { Name = exBlock.ExerciseName };
                        await db.InsertAsync(newEx);
                        exerciseDbId = newEx.Id;
                        exerciseNameToDbId[exBlock.ExerciseName] = newEx.Id;
                    }

                    var se = new SessionExercise
                    {
                        SessionId = session.Id,
                        ExerciseId = exerciseDbId,
                        Order = order++,
                        RestSeconds = exBlock.RestSeconds
                    };
                    await db.InsertAsync(se);

                    // Insert sets
                    foreach (var setData in exBlock.Sets)
                    {
                        var set = new Set
                        {
                            SessionExerciseId = se.Id,
                            SetNumber = setData.SetNumber,
                            IsWarmup = setData.IsWarmup,
                            RepMin = setData.RepMin,
                            RepMax = setData.RepMax,
                            DurationMin = setData.DurationMin,
                            DurationMax = setData.DurationMax,
                            PlannedWeight = setData.PlannedWeight,
                            Reps = setData.Reps,
                            Weight = setData.Weight,
                            DurationSeconds = setData.DurationSeconds,
                            Rpe = setData.Rpe,
                            Completed = setData.Reps.HasValue || setData.Weight.HasValue || setData.DurationSeconds.HasValue
                        };
                        await db.InsertAsync(set);
                    }
                }

                // Insert session tags
                if (i < xlsxSessions.Count && !string.IsNullOrWhiteSpace(xlsxSessions[i].Tags))
                {
                    foreach (var tagName in xlsxSessions[i].Tags!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    {
                        await db.InsertAsync(new SessionTag
                        {
                            SessionId = session.Id,
                            TagName = tagName
                        });
                    }
                }
            }

            return newProgram.Id;
        }
    }

    // ── XLSX Parsing Helpers ──

    private static Dictionary<string, string> ParseKeyValueSheet(IXLWorksheet sheet)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;
        for (int r = 2; r <= lastRow; r++)
        {
            var key = sheet.Cell(r, 1).GetString().Trim();
            var val = sheet.Cell(r, 2).GetString().Trim();
            if (!string.IsNullOrEmpty(key))
                dict[key] = val;
        }
        return dict;
    }

    private static List<XlsxExerciseData> ParseExercisesSheet(IXLWorksheet sheet)
    {
        var list = new List<XlsxExerciseData>();
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int r = 2; r <= lastRow; r++)
        {
            var name = sheet.Cell(r, 2).GetString().Trim();
            if (string.IsNullOrEmpty(name)) continue;

            int.TryParse(sheet.Cell(r, 1).GetString().Trim(), out var xlsxId);

            var exercise = new Exercise
            {
                Id = xlsxId,
                Name = name,
                Equipment = NullIfEmpty(sheet.Cell(r, 3).GetString().Trim()),
                ExerciseType = NullIfEmpty(sheet.Cell(r, 4).GetString().Trim()),
                IsTimeBased = sheet.Cell(r, 5).GetString().Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase),
                Description = NullIfEmpty(sheet.Cell(r, 6).GetString().Trim()),
                Instructions = NullIfEmpty(sheet.Cell(r, 7).GetString().Trim()),
                Notes = NullIfEmpty(sheet.Cell(r, 8).GetString().Trim()),
                ExampleMedia = NullIfEmpty(sheet.Cell(r, 11).GetString().Trim())
            };

            list.Add(new XlsxExerciseData
            {
                Exercise = exercise,
                PrimaryMuscles = sheet.Cell(r, 9).GetString().Trim(),
                SecondaryMuscles = sheet.Cell(r, 10).GetString().Trim()
            });
        }

        return list;
    }

    private static List<XlsxSessionData> ParseSessionsSheet(IXLWorksheet sheet)
    {
        var list = new List<XlsxSessionData>();
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int r = 2; r <= lastRow; r++)
        {
            var weekStr = sheet.Cell(r, 1).GetString().Trim();
            var dayStr = sheet.Cell(r, 2).GetString().Trim();
            var dateStr = sheet.Cell(r, 3).GetString().Trim();

            if (string.IsNullOrEmpty(dateStr) && string.IsNullOrEmpty(weekStr)) continue;

            list.Add(new XlsxSessionData
            {
                Week = int.TryParse(weekStr, out var w) ? w : null,
                Day = int.TryParse(dayStr, out var d) ? d : null,
                Date = DateTime.TryParse(dateStr, out var dt) ? dt : DateTime.Today,
                StartTime = TimeSpan.TryParse(sheet.Cell(r, 4).GetString().Trim(), out var st) ? st : null,
                EndTime = TimeSpan.TryParse(sheet.Cell(r, 5).GetString().Trim(), out var et) ? et : null,
                EnergyLevel = int.TryParse(sheet.Cell(r, 6).GetString().Trim(), out var el) ? el : null,
                Notes = NullIfEmpty(sheet.Cell(r, 7).GetString().Trim()),
                Completed = sheet.Cell(r, 8).GetString().Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase),
                Tags = NullIfEmpty(sheet.Cell(r, 9).GetString().Trim())
            });
        }

        return list;
    }

    private static List<WorkoutBlock> ParseWorkoutSheet(IXLWorksheet sheet)
    {
        var blocks = new List<WorkoutBlock>();
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;

        WorkoutBlock? currentBlock = null;
        WorkoutExerciseBlock? currentExercise = null;
        bool hasPlannedWtColumn = false;

        for (int r = 1; r <= lastRow; r++)
        {
            var cellA = sheet.Cell(r, 1);
            var cellAValue = cellA.GetString().Trim();

            // Session header: merged row with content
            if (cellA.IsMerged() && !string.IsNullOrEmpty(cellAValue))
            {
                currentBlock = new WorkoutBlock { HeaderText = cellAValue };
                blocks.Add(currentBlock);
                currentExercise = null;
                continue;
            }

            // Column header row — detect layout version and skip
            if (cellAValue.Equals("Exercise", StringComparison.OrdinalIgnoreCase))
            {
                var col6Header = sheet.Cell(r, 6).GetString().Trim();
                hasPlannedWtColumn = col6Header.Equals("PlannedWt", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            int totalCols = hasPlannedWtColumn ? 10 : 9;

            // Check for fully empty row
            if (IsRowEmpty(sheet, r, totalCols))
            {
                currentExercise = null;
                continue;
            }

            if (currentBlock == null) continue;

            // New exercise starts when column A has a value
            if (!string.IsNullOrEmpty(cellAValue))
            {
                var restStr = sheet.Cell(r, 2).GetString().Trim();
                currentExercise = new WorkoutExerciseBlock
                {
                    ExerciseName = cellAValue,
                    RestSeconds = int.TryParse(restStr, out var rest) ? rest : null
                };
                currentBlock.Exercises.Add(currentExercise);
            }

            if (currentExercise == null) continue;

            // Parse set
            var setStr = sheet.Cell(r, 3).GetString().Trim();
            if (!int.TryParse(setStr, out var setNum)) continue;

            var warmup = sheet.Cell(r, 4).GetString().Trim().Equals("W", StringComparison.OrdinalIgnoreCase);
            var planStr = sheet.Cell(r, 5).GetString().Trim();
            ParsePlan(planStr, out var repMin, out var repMax, out var durMin, out var durMax);

            double? plannedWeight = null;
            int repsCol, weightCol, durationCol, rpeCol;

            if (hasPlannedWtColumn)
            {
                var pwStr = sheet.Cell(r, 6).GetString().Trim();
                plannedWeight = double.TryParse(pwStr, out var pw) ? pw : null;
                repsCol = 7; weightCol = 8; durationCol = 9; rpeCol = 10;
            }
            else
            {
                repsCol = 6; weightCol = 7; durationCol = 8; rpeCol = 9;
            }

            var repsStr = sheet.Cell(r, repsCol).GetString().Trim();
            var weightStr = sheet.Cell(r, weightCol).GetString().Trim();
            var durationStr = sheet.Cell(r, durationCol).GetString().Trim();
            var rpeStr = sheet.Cell(r, rpeCol).GetString().Trim();

            currentExercise.Sets.Add(new WorkoutSetData
            {
                SetNumber = setNum,
                IsWarmup = warmup,
                RepMin = repMin,
                RepMax = repMax,
                DurationMin = durMin,
                DurationMax = durMax,
                PlannedWeight = plannedWeight,
                Reps = int.TryParse(repsStr, out var reps) ? reps : null,
                Weight = double.TryParse(weightStr, out var wt) ? wt : null,
                DurationSeconds = int.TryParse(durationStr, out var dur) ? dur : null,
                Rpe = double.TryParse(rpeStr, out var rpe) ? rpe : null
            });
        }

        return blocks;
    }

    private static void ParsePlan(string plan, out int? repMin, out int? repMax, out int? durMin, out int? durMax)
    {
        repMin = repMax = durMin = durMax = null;
        if (string.IsNullOrEmpty(plan)) return;

        bool isDuration = plan.EndsWith("s", StringComparison.OrdinalIgnoreCase);
        var cleanPlan = isDuration ? plan.TrimEnd('s', 'S') : plan;

        var parts = cleanPlan.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out var min) && int.TryParse(parts[1].Trim(), out var max))
        {
            if (isDuration) { durMin = min; durMax = max; }
            else { repMin = min; repMax = max; }
        }
        else if (int.TryParse(cleanPlan.Trim(), out var single))
        {
            if (isDuration) { durMin = single; durMax = single; }
            else { repMin = single; repMax = single; }
        }
    }

    private static void ParseHeaderWeekDay(string header, out int? week, out int? day)
    {
        week = day = null;
        // Pattern: "Week {W} - Day {D} | ..."
        var pipeIdx = header.IndexOf('|');
        var prefix = pipeIdx >= 0 ? header[..pipeIdx] : header;

        var parts = prefix.Split('-');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("Week", StringComparison.OrdinalIgnoreCase))
            {
                var numStr = trimmed["Week".Length..].Trim();
                if (int.TryParse(numStr, out var w)) week = w;
            }
            else if (trimmed.StartsWith("Day", StringComparison.OrdinalIgnoreCase))
            {
                var numStr = trimmed["Day".Length..].Trim();
                if (int.TryParse(numStr, out var d)) day = d;
            }
        }
    }

    private static bool IsRowEmpty(IXLWorksheet sheet, int row, int colCount)
    {
        for (int c = 1; c <= colCount; c++)
        {
            if (!string.IsNullOrEmpty(sheet.Cell(row, c).GetString().Trim()))
                return false;
        }
        return true;
    }

    private static async Task CreateMuscleLinkAsync(
        SQLite.SQLiteAsyncConnection db,
        int exerciseId,
        string muscleNames,
        string type,
        Dictionary<string, int> muscleNameToId)
    {
        if (string.IsNullOrWhiteSpace(muscleNames)) return;

        foreach (var name in muscleNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            int muscleId;
            if (muscleNameToId.TryGetValue(name, out var existingId))
            {
                muscleId = existingId;
            }
            else
            {
                var newMuscle = new Muscle { Name = name };
                await db.InsertAsync(newMuscle);
                muscleId = newMuscle.Id;
                muscleNameToId[name] = muscleId;
            }

            await db.InsertAsync(new ExerciseMuscle
            {
                ExerciseId = exerciseId,
                MuscleId = muscleId,
                Type = type
            });
        }
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    // ── Full Backup Import (JSON) ──

    public async Task ImportAsync(Stream stream)
    {
        ExportData? data;
        try
        {
            data = await JsonSerializer.DeserializeAsync<ExportData>(stream, JsonOptions);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("This file is not a valid backup file.");
        }

        var (isValid, errorMessage) = ValidateImport(data);
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        // Default null collections to empty
        var payload = data!.Data!;
        payload.Tags ??= [];
        payload.Muscles ??= [];
        payload.Exercises ??= [];
        payload.ExerciseMuscles ??= [];
        payload.Programs ??= [];
        payload.Sessions ??= [];
        payload.SessionExercises ??= [];
        payload.Sets ??= [];
        payload.BodyMetrics ??= [];
        payload.RecoveryLogs ??= [];
        payload.CalorieLogs ??= [];
        payload.SessionTags ??= [];

        var db = AppDatabase.Database;

        await db.RunInTransactionAsync(conn =>
        {
            // Delete all data (child tables first)
            conn.DeleteAll<SessionTag>();
            conn.DeleteAll<Set>();
            conn.DeleteAll<SessionExercise>();
            conn.DeleteAll<Session>();
            conn.DeleteAll<ExerciseMuscle>();
            conn.DeleteAll<Exercise>();
            conn.DeleteAll<Muscle>();
            conn.DeleteAll<Tag>();
            conn.DeleteAll<Program>();
            conn.DeleteAll<BodyMetric>();
            conn.DeleteAll<RecoveryLog>();
            conn.DeleteAll<CalorieLog>();

            // Insert all data (parent tables first, preserving original IDs).
            // "OR REPLACE" forces sqlite-net to include the PK column in the
            // INSERT so the exported IDs are kept and foreign-key references
            // (e.g. Session.ProgramId) remain valid.
            conn.InsertAll(payload.Tags, "OR REPLACE", false);
            conn.InsertAll(payload.Muscles, "OR REPLACE", false);
            conn.InsertAll(payload.Exercises, "OR REPLACE", false);
            conn.InsertAll(payload.ExerciseMuscles, "OR REPLACE", false);
            conn.InsertAll(payload.Programs, "OR REPLACE", false);
            conn.InsertAll(payload.Sessions, "OR REPLACE", false);
            conn.InsertAll(payload.SessionExercises, "OR REPLACE", false);
            conn.InsertAll(payload.Sets, "OR REPLACE", false);
            conn.InsertAll(payload.SessionTags, "OR REPLACE", false);
            conn.InsertAll(payload.BodyMetrics, "OR REPLACE", false);
            conn.InsertAll(payload.RecoveryLogs, "OR REPLACE", false);
            conn.InsertAll(payload.CalorieLogs, "OR REPLACE", false);
        });
    }

    // ── XLSX Import Helper Types ──

    private class XlsxExerciseData
    {
        public Exercise Exercise { get; set; } = new();
        public string PrimaryMuscles { get; set; } = "";
        public string SecondaryMuscles { get; set; } = "";
    }

    private class XlsxSessionData
    {
        public int? Week { get; set; }
        public int? Day { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? EnergyLevel { get; set; }
        public string? Notes { get; set; }
        public bool Completed { get; set; }
        public string? Tags { get; set; }
    }

    private class WorkoutBlock
    {
        public string HeaderText { get; set; } = "";
        public List<WorkoutExerciseBlock> Exercises { get; set; } = [];
    }

    private class WorkoutExerciseBlock
    {
        public string ExerciseName { get; set; } = "";
        public int? RestSeconds { get; set; }
        public List<WorkoutSetData> Sets { get; set; } = [];
    }

    private class WorkoutSetData
    {
        public int SetNumber { get; set; }
        public bool IsWarmup { get; set; }
        public int? RepMin { get; set; }
        public int? RepMax { get; set; }
        public int? DurationMin { get; set; }
        public int? DurationMax { get; set; }
        public double? PlannedWeight { get; set; }
        public int? Reps { get; set; }
        public double? Weight { get; set; }
        public int? DurationSeconds { get; set; }
        public double? Rpe { get; set; }
    }
}
