using Workout_Tracker.Model;
using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync()
    {
        await SeedMusclesAsync();
        await SeedExercisesAsync();
    }

    private static async Task SeedMusclesAsync()
    {
        var existingCount = await AppDatabase.Database.Table<Muscle>().CountAsync();
        if (existingCount > 0)
            return;

        var muscles = new List<Muscle>
        {
            // Neck
            new() { Name = "Neck Flexors" },
            new() { Name = "Neck Extensors" },

            // Chest
            new() { Name = "Upper Chest" },
            new() { Name = "Lower Chest" },

            // Back
            new() { Name = "Lats" },
            new() { Name = "Upper Traps" },
            new() { Name = "Middle Traps" },
            new() { Name = "Lower Traps" },
            new() { Name = "Rhomboids" },
            new() { Name = "Lower Back" },

            // Shoulders
            new() { Name = "Front Delts" },
            new() { Name = "Side Delts" },
            new() { Name = "Rear Delts" },
            new() { Name = "Rotator Cuff" },

            // Arms
            new() { Name = "Biceps" },
            new() { Name = "Triceps" },
            new() { Name = "Forearms" },

            // Core
            new() { Name = "Abs" },
            new() { Name = "Obliques" },
            new() { Name = "Serratus" },

            // Lower Body
            new() { Name = "Glutes" },
            new() { Name = "Quadriceps" },
            new() { Name = "Hamstrings" },
            new() { Name = "Hip Flexors" },
            new() { Name = "Adductors" },
            new() { Name = "Abductors" },
            new() { Name = "Calves" }
        };

        await AppDatabase.Database.InsertAllAsync(muscles);
    }

    private static async Task SeedExercisesAsync()
    {
        var existingCount = await AppDatabase.Database.Table<Exercise>().CountAsync();
        if (existingCount > 0)
            return;

        var exercises = new List<Exercise>
        {
            // ==================== NECK ====================
            new()
            {
                Name = "Neck Curl",
                Description = "Neck flexion exercise lying face up",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Neck Harness",
                IsTimeBased = false
            },
            new()
            {
                Name = "Reverse Neck Curl",
                Description = "Neck extension exercise lying face down",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Neck Harness",
                IsTimeBased = false
            },
            new()
            {
                Name = "Side Neck Curl",
                Description = "Lateral neck flexion exercise",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Neck Harness",
                IsTimeBased = false
            },

            // ==================== CHEST ====================
            new()
            {
                Name = "Bench Press",
                Description = "Barbell press lying on a flat bench",
                ExerciseType = "compound",
                Equipment = "Barbell, Bench",
                IsTimeBased = false
            },
            new()
            {
                Name = "Incline Bench Press",
                Description = "Barbell press on an inclined bench targeting upper chest",
                ExerciseType = "compound",
                Equipment = "Barbell, Incline Bench",
                IsTimeBased = false
            },
            new()
            {
                Name = "Dumbbell Fly",
                Description = "Chest isolation with dumbbells in an arc motion",
                ExerciseType = "isolation",
                Equipment = "Dumbbells, Bench",
                IsTimeBased = false
            },
            new()
            {
                Name = "Push-Up",
                Description = "Bodyweight chest press from the floor",
                ExerciseType = "compound",
                Equipment = "Bodyweight",
                IsTimeBased = false
            },
            new()
            {
                Name = "Cable Crossover",
                Description = "Cable fly movement for chest isolation",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Dips",
                Description = "Bodyweight pressing movement on parallel bars",
                ExerciseType = "compound",
                Equipment = "Dip Bars",
                IsTimeBased = false
            },

            // ==================== BACK ====================
            // Lats
            new()
            {
                Name = "Pull-Up",
                Description = "Bodyweight vertical pull with overhand grip",
                ExerciseType = "compound",
                Equipment = "Pull-Up Bar",
                IsTimeBased = false
            },
            new()
            {
                Name = "Chin-Up",
                Description = "Bodyweight vertical pull with underhand grip",
                ExerciseType = "compound",
                Equipment = "Pull-Up Bar",
                IsTimeBased = false
            },
            new()
            {
                Name = "Lat Pulldown",
                Description = "Cable pulldown targeting the lats",
                ExerciseType = "compound",
                Equipment = "Cable Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Barbell Row",
                Description = "Bent over rowing movement with barbell",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false
            },
            new()
            {
                Name = "Dumbbell Row",
                Description = "Single arm rowing movement with dumbbell",
                ExerciseType = "compound",
                Equipment = "Dumbbell, Bench",
                IsTimeBased = false
            },
            new()
            {
                Name = "Seated Cable Row",
                Description = "Horizontal pulling on cable machine",
                ExerciseType = "compound",
                Equipment = "Cable Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Ring Row",
                Description = "Horizontal pull using gymnastic rings at an angle",
                ExerciseType = "compound",
                Equipment = "Gymnastic Rings",
                IsTimeBased = false
            },
            // Traps
            new()
            {
                Name = "Shrugs",
                Description = "Shoulder shrug for trapezius",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Lower Trap Raise",
                Description = "Y-raise targeting the lower trapezius",
                ExerciseType = "isolation",
                Equipment = "Dumbbells or Cable",
                IsTimeBased = false
            },
            // Lower Back
            new()
            {
                Name = "Deadlift",
                Description = "Full body pull lifting barbell from the floor",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false
            },
            new()
            {
                Name = "Back Extension",
                Description = "Hyperextension movement for lower back on Roman chair or GHD",
                ExerciseType = "isolation",
                Equipment = "Roman Chair or GHD",
                IsTimeBased = false
            },

            // ==================== SHOULDERS ====================
            // Front Delts
            new()
            {
                Name = "Overhead Press",
                Description = "Standing barbell press overhead",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false
            },
            new()
            {
                Name = "Dumbbell Shoulder Press",
                Description = "Seated or standing dumbbell press overhead",
                ExerciseType = "compound",
                Equipment = "Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Front Raise",
                Description = "Dumbbell raise to the front for front delts",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false
            },
            // Side Delts
            new()
            {
                Name = "Lateral Raise",
                Description = "Dumbbell raise to the side for side delts",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Incline Lateral Raise",
                Description = "Lateral raise performed on an incline bench",
                ExerciseType = "isolation",
                Equipment = "Dumbbells, Incline Bench",
                IsTimeBased = false
            },
            // Rear Delts
            new()
            {
                Name = "Reverse Fly",
                Description = "Bent over fly for rear delts",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Face Pull",
                Description = "Cable pull to face targeting rear delts and upper back",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Band Face Pull",
                Description = "Face pull using resistance band",
                ExerciseType = "isolation",
                Equipment = "Resistance Band",
                IsTimeBased = false
            },
            new()
            {
                Name = "Ring Face Pull",
                Description = "Face pull using gymnastic rings",
                ExerciseType = "isolation",
                Equipment = "Gymnastic Rings",
                IsTimeBased = false
            },
            // Rotator Cuff
            new()
            {
                Name = "Band External Rotation",
                Description = "External rotation with resistance band for rotator cuff",
                ExerciseType = "isolation",
                Equipment = "Resistance Band",
                IsTimeBased = false
            },
            new()
            {
                Name = "Band Internal Rotation",
                Description = "Internal rotation with resistance band for rotator cuff",
                ExerciseType = "isolation",
                Equipment = "Resistance Band",
                IsTimeBased = false
            },

            // ==================== ARMS - BICEPS ====================
            new()
            {
                Name = "Barbell Curl",
                Description = "Standing curl with barbell",
                ExerciseType = "isolation",
                Equipment = "Barbell",
                IsTimeBased = false
            },
            new()
            {
                Name = "Dumbbell Curl",
                Description = "Standing or seated curl with dumbbells",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Hammer Curl",
                Description = "Neutral grip dumbbell curl",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Preacher Curl",
                Description = "Curl on preacher bench for bicep isolation",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells, Preacher Bench",
                IsTimeBased = false
            },

            // ==================== ARMS - TRICEPS ====================
            new()
            {
                Name = "Tricep Pushdown",
                Description = "Cable pushdown for triceps",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Skull Crusher",
                Description = "Lying tricep extension with barbell or dumbbells",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells, Bench",
                IsTimeBased = false
            },
            new()
            {
                Name = "Overhead Tricep Extension",
                Description = "Tricep extension overhead with dumbbell or cable",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Cable",
                IsTimeBased = false
            },
            new()
            {
                Name = "Close Grip Bench Press",
                Description = "Bench press with narrow grip for triceps",
                ExerciseType = "compound",
                Equipment = "Barbell, Bench",
                IsTimeBased = false
            },

            // ==================== ARMS - FOREARMS ====================
            new()
            {
                Name = "Wrist Curl",
                Description = "Forearm flexion with barbell or dumbbells",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Reverse Wrist Curl",
                Description = "Forearm extension with barbell or dumbbells",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Plate Pinch",
                Description = "Grip exercise pinching weight plates together",
                ExerciseType = "isolation",
                Equipment = "Weight Plates",
                IsTimeBased = true
            },
            new()
            {
                Name = "Dead Hang",
                Description = "Hanging from bar for grip and shoulder health",
                ExerciseType = "isolation",
                Equipment = "Pull-Up Bar",
                IsTimeBased = true
            },
            new()
            {
                Name = "Radial Deviation",
                Description = "Wrist movement toward thumb side with weight",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Hammer",
                IsTimeBased = false
            },
            new()
            {
                Name = "Ulnar Deviation",
                Description = "Wrist movement toward pinky side with weight",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Hammer",
                IsTimeBased = false
            },

            // ==================== CORE ====================
            // Abs
            new()
            {
                Name = "Plank",
                Description = "Isometric core hold in push-up position",
                ExerciseType = "isolation",
                Equipment = "Bodyweight",
                IsTimeBased = true
            },
            new()
            {
                Name = "Crunch",
                Description = "Basic abdominal crunch",
                ExerciseType = "isolation",
                Equipment = "Bodyweight",
                IsTimeBased = false
            },
            new()
            {
                Name = "Hanging Leg Raise",
                Description = "Leg raise while hanging from bar",
                ExerciseType = "isolation",
                Equipment = "Pull-Up Bar",
                IsTimeBased = false
            },
            new()
            {
                Name = "Ab Wheel Rollout",
                Description = "Rolling extension with ab wheel",
                ExerciseType = "isolation",
                Equipment = "Ab Wheel",
                IsTimeBased = false
            },
            new()
            {
                Name = "Dragon Flag",
                Description = "Advanced core exercise lying on bench with body extended",
                ExerciseType = "isolation",
                Equipment = "Bench",
                IsTimeBased = false
            },
            // Obliques
            new()
            {
                Name = "Russian Twist",
                Description = "Rotational core exercise",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Weight Plate",
                IsTimeBased = false
            },
            new()
            {
                Name = "Cable Woodchop",
                Description = "Rotational movement with cable",
                ExerciseType = "compound",
                Equipment = "Cable Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Anti-Rotation Hold",
                Description = "Pallof press hold for core anti-rotation stability",
                ExerciseType = "isolation",
                Equipment = "Cable Machine or Band",
                IsTimeBased = true
            },
            new()
            {
                Name = "Side Back Extension",
                Description = "Lateral back extension targeting obliques and lower back",
                ExerciseType = "isolation",
                Equipment = "Roman Chair or GHD",
                IsTimeBased = false
            },

            // ==================== LOWER BODY ====================
            // Glutes
            new()
            {
                Name = "Hip Thrust",
                Description = "Glute bridge with back on bench",
                ExerciseType = "compound",
                Equipment = "Barbell, Bench",
                IsTimeBased = false
            },
            // Quadriceps
            new()
            {
                Name = "Squat",
                Description = "Barbell back squat",
                ExerciseType = "compound",
                Equipment = "Barbell, Squat Rack",
                IsTimeBased = false
            },
            new()
            {
                Name = "Front Squat",
                Description = "Barbell squat with bar in front rack position",
                ExerciseType = "compound",
                Equipment = "Barbell, Squat Rack",
                IsTimeBased = false
            },
            new()
            {
                Name = "Bodyweight Squat",
                Description = "Air squat with no added weight",
                ExerciseType = "compound",
                Equipment = "Bodyweight",
                IsTimeBased = false
            },
            new()
            {
                Name = "Leg Press",
                Description = "Machine press for quadriceps and glutes",
                ExerciseType = "compound",
                Equipment = "Leg Press Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Leg Extension",
                Description = "Machine extension for quadriceps",
                ExerciseType = "isolation",
                Equipment = "Leg Extension Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Lunges",
                Description = "Stepping lunge with bodyweight or weights",
                ExerciseType = "compound",
                Equipment = "Bodyweight or Dumbbells",
                IsTimeBased = false
            },
            new()
            {
                Name = "Bulgarian Split Squat",
                Description = "Single leg squat with rear foot elevated",
                ExerciseType = "compound",
                Equipment = "Dumbbells, Bench",
                IsTimeBased = false
            },
            // Hamstrings
            new()
            {
                Name = "Romanian Deadlift",
                Description = "Hip hinge movement targeting hamstrings",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false
            },
            new()
            {
                Name = "Leg Curl",
                Description = "Machine curl for hamstrings",
                ExerciseType = "isolation",
                Equipment = "Leg Curl Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Nordic Curl",
                Description = "Eccentric hamstring curl with bodyweight",
                ExerciseType = "isolation",
                Equipment = "Nordic Curl Station or Partner",
                IsTimeBased = false
            },
            // Hip Flexors
            new()
            {
                Name = "Hip Flexor Raise",
                Description = "Raising the knee against gravity for hip flexor strength",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Ankle Weights",
                IsTimeBased = false
            },
            // Adductors
            new()
            {
                Name = "Hip Adductor",
                Description = "Machine adduction for inner thighs",
                ExerciseType = "isolation",
                Equipment = "Adductor Machine",
                IsTimeBased = false
            },
            new()
            {
                Name = "Cossack Squat",
                Description = "Side-to-side squat for adductors and mobility",
                ExerciseType = "compound",
                Equipment = "Bodyweight or Dumbbells",
                IsTimeBased = false
            },
            // Abductors
            new()
            {
                Name = "Hip Abductor",
                Description = "Machine abduction for outer hips",
                ExerciseType = "isolation",
                Equipment = "Abductor Machine",
                IsTimeBased = false
            },
            // Calves
            new()
            {
                Name = "Calf Raise",
                Description = "Standing or seated calf raise",
                ExerciseType = "isolation",
                Equipment = "Machine or Bodyweight",
                IsTimeBased = false
            },
            new()
            {
                Name = "Reverse Calf Raise",
                Description = "Tibialis raise for shin muscles",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Tibialis Machine",
                IsTimeBased = false
            },

            // ==================== CARDIO ====================
            new()
            {
                Name = "Running",
                Description = "Outdoor or treadmill running",
                ExerciseType = "cardio",
                Equipment = "Treadmill or Outdoors",
                IsTimeBased = true
            },
            new()
            {
                Name = "Cycling",
                Description = "Stationary bike or outdoor cycling",
                ExerciseType = "cardio",
                Equipment = "Bike",
                IsTimeBased = true
            },
            new()
            {
                Name = "Rowing",
                Description = "Rowing machine cardio",
                ExerciseType = "cardio",
                Equipment = "Rowing Machine",
                IsTimeBased = true
            },
            new()
            {
                Name = "Jump Rope",
                Description = "Skipping rope cardio",
                ExerciseType = "cardio",
                Equipment = "Jump Rope",
                IsTimeBased = true
            },

            // ==================== MOBILITY ====================
            new()
            {
                Name = "Foam Rolling",
                Description = "Self-myofascial release with foam roller",
                ExerciseType = "mobility",
                Equipment = "Foam Roller",
                IsTimeBased = true
            },
            new()
            {
                Name = "Hip Flexor Stretch",
                Description = "Stretching the hip flexor muscles",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true
            },
            new()
            {
                Name = "Shoulder Dislocates",
                Description = "Shoulder mobility with band or stick",
                ExerciseType = "mobility",
                Equipment = "Band or Stick",
                IsTimeBased = false
            },
            new()
            {
                Name = "Pigeon Stretch",
                Description = "Hip opener stretch targeting glutes and hip rotators",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true
            },
            new()
            {
                Name = "Leg Swings",
                Description = "Dynamic leg swings for hip mobility warmup",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = false
            },
            new()
            {
                Name = "Hip External Rotation Hold",
                Description = "Isometric hold in external hip rotation",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true
            },
            new()
            {
                Name = "Hip Internal Rotation Hold",
                Description = "Isometric hold in internal hip rotation",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true
            }
        };

        await AppDatabase.Database.InsertAllAsync(exercises);

        // Seed exercise-muscle relationships
        await SeedExerciseMusclesAsync();
    }

    private static async Task SeedExerciseMusclesAsync()
    {
        var muscles = await AppDatabase.Database.Table<Muscle>().ToListAsync();
        var exercises = await AppDatabase.Database.Table<Exercise>().ToListAsync();

        var muscleDict = muscles.ToDictionary(m => m.Name, m => m.Id);
        var exerciseDict = exercises.ToDictionary(e => e.Name, e => e.Id);

        var exerciseMuscles = new List<ExerciseMuscle>();

        void AddMapping(string exercise, string muscle, string type)
        {
            if (exerciseDict.TryGetValue(exercise, out int exId) &&
                muscleDict.TryGetValue(muscle, out int muscleId))
            {
                exerciseMuscles.Add(new ExerciseMuscle
                {
                    ExerciseId = exId,
                    MuscleId = muscleId,
                    Type = type
                });
            }
        }

        // ==================== NECK ====================
        AddMapping("Neck Curl", "Neck Flexors", "primary");
        AddMapping("Reverse Neck Curl", "Neck Extensors", "primary");
        AddMapping("Side Neck Curl", "Neck Flexors", "primary");
        AddMapping("Side Neck Curl", "Neck Extensors", "secondary");

        // ==================== CHEST ====================
        AddMapping("Bench Press", "Upper Chest", "primary");
        AddMapping("Bench Press", "Lower Chest", "primary");
        AddMapping("Bench Press", "Triceps", "secondary");
        AddMapping("Bench Press", "Front Delts", "secondary");

        AddMapping("Incline Bench Press", "Upper Chest", "primary");
        AddMapping("Incline Bench Press", "Triceps", "secondary");
        AddMapping("Incline Bench Press", "Front Delts", "secondary");

        AddMapping("Dumbbell Fly", "Upper Chest", "primary");
        AddMapping("Dumbbell Fly", "Lower Chest", "primary");

        AddMapping("Push-Up", "Upper Chest", "primary");
        AddMapping("Push-Up", "Lower Chest", "primary");
        AddMapping("Push-Up", "Triceps", "secondary");
        AddMapping("Push-Up", "Front Delts", "secondary");

        AddMapping("Cable Crossover", "Upper Chest", "primary");
        AddMapping("Cable Crossover", "Lower Chest", "primary");

        AddMapping("Dips", "Lower Chest", "primary");
        AddMapping("Dips", "Triceps", "primary");
        AddMapping("Dips", "Front Delts", "secondary");

        // ==================== BACK ====================
        // Lats
        AddMapping("Pull-Up", "Lats", "primary");
        AddMapping("Pull-Up", "Biceps", "secondary");
        AddMapping("Pull-Up", "Rhomboids", "secondary");

        AddMapping("Chin-Up", "Lats", "primary");
        AddMapping("Chin-Up", "Biceps", "primary");
        AddMapping("Chin-Up", "Rhomboids", "secondary");

        AddMapping("Lat Pulldown", "Lats", "primary");
        AddMapping("Lat Pulldown", "Biceps", "secondary");

        AddMapping("Barbell Row", "Lats", "primary");
        AddMapping("Barbell Row", "Rhomboids", "primary");
        AddMapping("Barbell Row", "Middle Traps", "secondary");
        AddMapping("Barbell Row", "Biceps", "secondary");

        AddMapping("Dumbbell Row", "Lats", "primary");
        AddMapping("Dumbbell Row", "Rhomboids", "secondary");
        AddMapping("Dumbbell Row", "Biceps", "secondary");

        AddMapping("Seated Cable Row", "Lats", "primary");
        AddMapping("Seated Cable Row", "Rhomboids", "primary");
        AddMapping("Seated Cable Row", "Biceps", "secondary");

        AddMapping("Ring Row", "Lats", "primary");
        AddMapping("Ring Row", "Rhomboids", "primary");
        AddMapping("Ring Row", "Biceps", "secondary");
        AddMapping("Ring Row", "Rear Delts", "secondary");

        // Traps
        AddMapping("Shrugs", "Upper Traps", "primary");
        AddMapping("Lower Trap Raise", "Lower Traps", "primary");

        // Lower Back
        AddMapping("Deadlift", "Lower Back", "primary");
        AddMapping("Deadlift", "Glutes", "primary");
        AddMapping("Deadlift", "Hamstrings", "primary");
        AddMapping("Deadlift", "Upper Traps", "secondary");
        AddMapping("Deadlift", "Forearms", "secondary");

        AddMapping("Back Extension", "Lower Back", "primary");
        AddMapping("Back Extension", "Glutes", "secondary");
        AddMapping("Back Extension", "Hamstrings", "secondary");

        // ==================== SHOULDERS ====================
        // Front Delts
        AddMapping("Overhead Press", "Front Delts", "primary");
        AddMapping("Overhead Press", "Side Delts", "secondary");
        AddMapping("Overhead Press", "Triceps", "secondary");

        AddMapping("Dumbbell Shoulder Press", "Front Delts", "primary");
        AddMapping("Dumbbell Shoulder Press", "Side Delts", "secondary");
        AddMapping("Dumbbell Shoulder Press", "Triceps", "secondary");

        AddMapping("Front Raise", "Front Delts", "primary");

        // Side Delts
        AddMapping("Lateral Raise", "Side Delts", "primary");
        AddMapping("Incline Lateral Raise", "Side Delts", "primary");

        // Rear Delts
        AddMapping("Reverse Fly", "Rear Delts", "primary");
        AddMapping("Reverse Fly", "Rhomboids", "secondary");

        AddMapping("Face Pull", "Rear Delts", "primary");
        AddMapping("Face Pull", "Middle Traps", "primary");
        AddMapping("Face Pull", "Rotator Cuff", "secondary");

        AddMapping("Band Face Pull", "Rear Delts", "primary");
        AddMapping("Band Face Pull", "Middle Traps", "secondary");
        AddMapping("Band Face Pull", "Rotator Cuff", "secondary");

        AddMapping("Ring Face Pull", "Rear Delts", "primary");
        AddMapping("Ring Face Pull", "Middle Traps", "secondary");
        AddMapping("Ring Face Pull", "Rotator Cuff", "secondary");

        // Rotator Cuff
        AddMapping("Band External Rotation", "Rotator Cuff", "primary");
        AddMapping("Band Internal Rotation", "Rotator Cuff", "primary");

        // ==================== ARMS - BICEPS ====================
        AddMapping("Barbell Curl", "Biceps", "primary");
        AddMapping("Barbell Curl", "Forearms", "secondary");

        AddMapping("Dumbbell Curl", "Biceps", "primary");
        AddMapping("Dumbbell Curl", "Forearms", "secondary");

        AddMapping("Hammer Curl", "Biceps", "primary");
        AddMapping("Hammer Curl", "Forearms", "primary");

        AddMapping("Preacher Curl", "Biceps", "primary");

        // ==================== ARMS - TRICEPS ====================
        AddMapping("Tricep Pushdown", "Triceps", "primary");
        AddMapping("Skull Crusher", "Triceps", "primary");
        AddMapping("Overhead Tricep Extension", "Triceps", "primary");

        AddMapping("Close Grip Bench Press", "Triceps", "primary");
        AddMapping("Close Grip Bench Press", "Upper Chest", "secondary");
        AddMapping("Close Grip Bench Press", "Front Delts", "secondary");

        // ==================== ARMS - FOREARMS ====================
        AddMapping("Wrist Curl", "Forearms", "primary");
        AddMapping("Reverse Wrist Curl", "Forearms", "primary");
        AddMapping("Plate Pinch", "Forearms", "primary");
        AddMapping("Dead Hang", "Forearms", "primary");
        AddMapping("Dead Hang", "Lats", "secondary");
        AddMapping("Radial Deviation", "Forearms", "primary");
        AddMapping("Ulnar Deviation", "Forearms", "primary");

        // ==================== CORE ====================
        // Abs
        AddMapping("Plank", "Abs", "primary");
        AddMapping("Plank", "Obliques", "secondary");

        AddMapping("Crunch", "Abs", "primary");

        AddMapping("Hanging Leg Raise", "Abs", "primary");
        AddMapping("Hanging Leg Raise", "Hip Flexors", "secondary");

        AddMapping("Ab Wheel Rollout", "Abs", "primary");
        AddMapping("Ab Wheel Rollout", "Serratus", "secondary");

        AddMapping("Dragon Flag", "Abs", "primary");
        AddMapping("Dragon Flag", "Hip Flexors", "secondary");
        AddMapping("Dragon Flag", "Obliques", "secondary");

        // Obliques
        AddMapping("Russian Twist", "Obliques", "primary");
        AddMapping("Russian Twist", "Abs", "secondary");

        AddMapping("Cable Woodchop", "Obliques", "primary");
        AddMapping("Cable Woodchop", "Abs", "secondary");

        AddMapping("Anti-Rotation Hold", "Obliques", "primary");
        AddMapping("Anti-Rotation Hold", "Abs", "secondary");

        AddMapping("Side Back Extension", "Obliques", "primary");
        AddMapping("Side Back Extension", "Lower Back", "secondary");

        // ==================== LOWER BODY ====================
        // Glutes
        AddMapping("Hip Thrust", "Glutes", "primary");
        AddMapping("Hip Thrust", "Hamstrings", "secondary");

        // Quadriceps
        AddMapping("Squat", "Quadriceps", "primary");
        AddMapping("Squat", "Glutes", "primary");
        AddMapping("Squat", "Lower Back", "secondary");
        AddMapping("Squat", "Hamstrings", "secondary");

        AddMapping("Front Squat", "Quadriceps", "primary");
        AddMapping("Front Squat", "Glutes", "secondary");
        AddMapping("Front Squat", "Abs", "secondary");

        AddMapping("Bodyweight Squat", "Quadriceps", "primary");
        AddMapping("Bodyweight Squat", "Glutes", "secondary");

        AddMapping("Leg Press", "Quadriceps", "primary");
        AddMapping("Leg Press", "Glutes", "secondary");

        AddMapping("Leg Extension", "Quadriceps", "primary");

        AddMapping("Lunges", "Quadriceps", "primary");
        AddMapping("Lunges", "Glutes", "primary");
        AddMapping("Lunges", "Hamstrings", "secondary");

        AddMapping("Bulgarian Split Squat", "Quadriceps", "primary");
        AddMapping("Bulgarian Split Squat", "Glutes", "primary");
        AddMapping("Bulgarian Split Squat", "Hamstrings", "secondary");

        // Hamstrings
        AddMapping("Romanian Deadlift", "Hamstrings", "primary");
        AddMapping("Romanian Deadlift", "Glutes", "primary");
        AddMapping("Romanian Deadlift", "Lower Back", "secondary");

        AddMapping("Leg Curl", "Hamstrings", "primary");
        AddMapping("Nordic Curl", "Hamstrings", "primary");

        // Hip Flexors
        AddMapping("Hip Flexor Raise", "Hip Flexors", "primary");

        // Adductors
        AddMapping("Hip Adductor", "Adductors", "primary");

        AddMapping("Cossack Squat", "Adductors", "primary");
        AddMapping("Cossack Squat", "Quadriceps", "secondary");
        AddMapping("Cossack Squat", "Glutes", "secondary");

        // Abductors
        AddMapping("Hip Abductor", "Abductors", "primary");

        // Calves
        AddMapping("Calf Raise", "Calves", "primary");
        AddMapping("Reverse Calf Raise", "Calves", "primary");

        await AppDatabase.Database.InsertAllAsync(exerciseMuscles);
    }
}
