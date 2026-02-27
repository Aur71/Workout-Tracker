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
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=FPO1wKDQUs8"
            },
            new()
            {
                Name = "Reverse Neck Curl",
                Description = "Neck extension exercise lying face down",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Neck Harness",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=393tGWUXCXg"
            },
            new()
            {
                Name = "Side Neck Curl",
                Description = "Lateral neck flexion exercise",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Neck Harness",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=FBUiuhB10ic"
            },

            // ==================== CHEST ====================
            new()
            {
                Name = "Bench Press",
                Description = "Barbell press lying on a flat bench",
                ExerciseType = "compound",
                Equipment = "Barbell, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=SCVCLChPQFY"
            },
            new()
            {
                Name = "Incline Bench Press",
                Description = "Barbell press on an inclined bench targeting upper chest",
                ExerciseType = "compound",
                Equipment = "Barbell, Incline Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=lJ2o89kcnxY"
            },
            new()
            {
                Name = "Dumbbell Fly",
                Description = "Chest isolation with dumbbells in an arc motion",
                ExerciseType = "isolation",
                Equipment = "Dumbbells, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=JFm8KbhjibM"
            },
            new()
            {
                Name = "Push-Up",
                Description = "Bodyweight chest press from the floor",
                ExerciseType = "compound",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=WDIpL0pjun0"
            },
            new()
            {
                Name = "Decline Push-Up",
                Description = "Push-up with feet elevated on a bench, emphasizing upper chest and front delts",
                ExerciseType = "compound",
                Equipment = "Bodyweight, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=SKPab2YC8BE"
            },
            new()
            {
                Name = "Bulgarian Push-Up",
                Description = "Push-up with feet elevated on a bench and hands on the floor, allowing a deeper range of motion for greater chest stretch",
                ExerciseType = "compound",
                Equipment = "Bodyweight, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=X5HojVeu2w4"
            },
            new()
            {
                Name = "Decline Bulgarian Push-Up",
                Description = "Bulgarian push-up performed with feet elevated higher than hands, combining the deep stretch of Bulgarian push-ups with the upper chest emphasis of decline push-ups",
                ExerciseType = "compound",
                Equipment = "Bodyweight, Two Benches",
                IsTimeBased = false
            },
            new()
            {
                Name = "Cable Crossover",
                Description = "Cable fly movement for chest isolation",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=EsXgqAxVKbA"
            },
            new()
            {
                Name = "Dips",
                Description = "Bodyweight pressing movement on parallel bars",
                ExerciseType = "compound",
                Equipment = "Dip Bars",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=2hnLB6O860c"
            },

            // ==================== BACK ====================
            // Lats
            new()
            {
                Name = "Pull-Up",
                Description = "Bodyweight vertical pull with overhand grip",
                ExerciseType = "compound",
                Equipment = "Pull-Up Bar",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=eGo4IYlbE5g"
            },
            new()
            {
                Name = "Chin-Up",
                Description = "Bodyweight vertical pull with underhand grip",
                ExerciseType = "compound",
                Equipment = "Pull-Up Bar",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=8mryJ3w2S78"
            },
            new()
            {
                Name = "Lat Pulldown",
                Description = "Cable pulldown targeting the lats",
                ExerciseType = "compound",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=JGeRYIZdojU"
            },
            new()
            {
                Name = "Barbell Row",
                Description = "Bent over rowing movement with barbell",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=bm0_q9bR_HA"
            },
            new()
            {
                Name = "Dumbbell Row",
                Description = "Single arm rowing movement with dumbbell",
                ExerciseType = "compound",
                Equipment = "Dumbbell, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=DMo3HJoawrU"
            },
            new()
            {
                Name = "Seated Cable Row",
                Description = "Horizontal pulling on cable machine",
                ExerciseType = "compound",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=UCXxvVItLoM"
            },
            new()
            {
                Name = "Ring Row",
                Description = "Horizontal pull using gymnastic rings at an angle",
                ExerciseType = "compound",
                Equipment = "Gymnastic Rings",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=B90sF7dbP04"
            },
            // Traps
            new()
            {
                Name = "Shrugs",
                Description = "Shoulder shrug for trapezius",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=_t3lrPI6Ns4"
            },
            new()
            {
                Name = "Lower Trap Raise",
                Description = "Y-raise targeting the lower trapezius",
                ExerciseType = "isolation",
                Equipment = "Dumbbells or Cable",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=b9Hkd4DlRes"
            },
            // Lower Back
            new()
            {
                Name = "Deadlift",
                Description = "Full body pull lifting barbell from the floor",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=AweC3UaM14o"
            },
            new()
            {
                Name = "Back Extension",
                Description = "Hyperextension movement for lower back on Roman chair or GHD",
                ExerciseType = "isolation",
                Equipment = "Roman Chair or GHD",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=7NHiEO5h4c8"
            },

            // ==================== SHOULDERS ====================
            // Front Delts
            new()
            {
                Name = "Overhead Press",
                Description = "Standing barbell press overhead",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=ZXpdJOLNoWw"
            },
            new()
            {
                Name = "Dumbbell Shoulder Press",
                Description = "Seated or standing dumbbell press overhead",
                ExerciseType = "compound",
                Equipment = "Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=HzIiNhHhhtA"
            },
            new()
            {
                Name = "Front Raise",
                Description = "Dumbbell raise to the front for front delts",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=t8asfRMKd-M"
            },
            // Side Delts
            new()
            {
                Name = "Lateral Raise",
                Description = "Dumbbell raise to the side for side delts",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=XPPfnSEATJA"
            },
            new()
            {
                Name = "Incline Lateral Raise",
                Description = "Lateral raise performed on an incline bench",
                ExerciseType = "isolation",
                Equipment = "Dumbbells, Incline Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=sLhYbdTZodU"
            },
            // Rear Delts
            new()
            {
                Name = "Reverse Fly",
                Description = "Bent over fly for rear delts",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=8wN98DY8FD4"
            },
            new()
            {
                Name = "Face Pull",
                Description = "Cable pull to face targeting rear delts and upper back",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=-MODnZdnmAQ"
            },
            new()
            {
                Name = "Band Face Pull",
                Description = "Face pull using resistance band",
                ExerciseType = "isolation",
                Equipment = "Resistance Band",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=hbo-nSIEmXo"
            },
            new()
            {
                Name = "Ring Face Pull",
                Description = "Face pull using gymnastic rings",
                ExerciseType = "isolation",
                Equipment = "Gymnastic Rings",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=yuhwEvC-XAU"
            },
            // Rotator Cuff
            new()
            {
                Name = "Band External Rotation",
                Description = "External rotation with resistance band for rotator cuff",
                ExerciseType = "isolation",
                Equipment = "Resistance Band",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=dLSytuFOCX8"
            },
            new()
            {
                Name = "Band Internal Rotation",
                Description = "Internal rotation with resistance band for rotator cuff",
                ExerciseType = "isolation",
                Equipment = "Resistance Band",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=X6EDIjqgPS4"
            },

            // ==================== ARMS - BICEPS ====================
            new()
            {
                Name = "Barbell Curl",
                Description = "Standing curl with barbell",
                ExerciseType = "isolation",
                Equipment = "Barbell",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=v_rZqV01aiY"
            },
            new()
            {
                Name = "Dumbbell Curl",
                Description = "Standing or seated curl with dumbbells",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=W3GrFfEWBJM"
            },
            new()
            {
                Name = "Hammer Curl",
                Description = "Neutral grip dumbbell curl",
                ExerciseType = "isolation",
                Equipment = "Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=CFBZ4jN1CMI"
            },
            new()
            {
                Name = "Preacher Curl",
                Description = "Curl on preacher bench for bicep isolation",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells, Preacher Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=Ja6ZlIDONac"
            },

            // ==================== ARMS - TRICEPS ====================
            new()
            {
                Name = "Tricep Pushdown",
                Description = "Cable pushdown for triceps",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=6Fzep104f0s"
            },
            new()
            {
                Name = "Skull Crusher",
                Description = "Lying tricep extension with barbell or dumbbells",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=l3rHYPtMUo8"
            },
            new()
            {
                Name = "Overhead Tricep Extension",
                Description = "Tricep extension overhead with dumbbell or cable",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Cable",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=1u18yJELsh0"
            },
            new()
            {
                Name = "Close Grip Bench Press",
                Description = "Bench press with narrow grip for triceps",
                ExerciseType = "compound",
                Equipment = "Barbell, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=FiQUzPtS90E"
            },

            new()
            {
                Name = "Single Arm Cross Body Tricep Extension",
                Description = "Cable tricep extension pulling across the body for long head emphasis",
                ExerciseType = "isolation",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=b_ITzRs_Jbo"
            },

            // ==================== ARMS - FOREARMS ====================
            new()
            {
                Name = "Wrist Rolls",
                Description = "Rolling a wrist roller to wind up a weight for forearm strength",
                ExerciseType = "isolation",
                Equipment = "Wrist Roller",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=C4urEGe0zsQ"
            },
            new()
            {
                Name = "Reverse Wrist Rolls",
                Description = "Reverse rolling a wrist roller to unwind a weight for forearm extension strength",
                ExerciseType = "isolation",
                Equipment = "Wrist Roller",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=Wl1DxBK8yRo"
            },
            new()
            {
                Name = "Hand Supination",
                Description = "Rotating the forearm to turn the palm upward against resistance",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Hammer",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=r7caPXvhINc"
            },
            new()
            {
                Name = "Hand Pronation",
                Description = "Rotating the forearm to turn the palm downward against resistance",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Hammer",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=r7caPXvhINc"
            },
            new()
            {
                Name = "Wrist Curl",
                Description = "Forearm flexion with barbell or dumbbells",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=3VLTzIrnb5g"
            },
            new()
            {
                Name = "Reverse Wrist Curl",
                Description = "Forearm extension with barbell or dumbbells",
                ExerciseType = "isolation",
                Equipment = "Barbell or Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=osYPwlBiCRM"
            },
            new()
            {
                Name = "Plate Pinch",
                Description = "Grip exercise pinching weight plates together",
                ExerciseType = "isolation",
                Equipment = "Weight Plates",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/watch?v=jFTV3DQf3HE"
            },
            new()
            {
                Name = "Dead Hang",
                Description = "Hanging from bar for grip and shoulder health",
                ExerciseType = "isolation",
                Equipment = "Pull-Up Bar",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/watch?v=0Bx_Ap7-EwU"
            },
            new()
            {
                Name = "Radial Deviation",
                Description = "Wrist movement toward thumb side with weight",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Hammer",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=_B61Ym6kKYM"
            },
            new()
            {
                Name = "Ulnar Deviation",
                Description = "Wrist movement toward pinky side with weight",
                ExerciseType = "isolation",
                Equipment = "Dumbbell or Hammer",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=_B61Ym6kKYM"
            },

            // ==================== CORE ====================
            // Abs
            new()
            {
                Name = "Plank",
                Description = "Isometric core hold in push-up position",
                ExerciseType = "isolation",
                Equipment = "Bodyweight",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/watch?v=pvIjsG5Svck"
            },
            new()
            {
                Name = "Crunch",
                Description = "Basic abdominal crunch",
                ExerciseType = "isolation",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=Xyd_fa5zoEU"
            },
            new()
            {
                Name = "Hanging Leg Raise",
                Description = "Leg raise while hanging from bar",
                ExerciseType = "isolation",
                Equipment = "Pull-Up Bar",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=7FwGZ8qY5OU"
            },
            new()
            {
                Name = "Ab Wheel Rollout",
                Description = "Rolling extension with ab wheel",
                ExerciseType = "isolation",
                Equipment = "Ab Wheel",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=3b8K6BCvBBA"
            },
            new()
            {
                Name = "Dragon Flag",
                Description = "Advanced core exercise lying on bench with body extended",
                ExerciseType = "isolation",
                Equipment = "Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=U5pviWt7sMo"
            },
            // Obliques
            new()
            {
                Name = "Russian Twist",
                Description = "Rotational core exercise",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Weight Plate",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=DJQGX2J4IVw"
            },
            new()
            {
                Name = "Cable Woodchop",
                Description = "Rotational movement with cable",
                ExerciseType = "compound",
                Equipment = "Cable Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=iWxTGXIViro"
            },
            new()
            {
                Name = "Anti-Rotation Hold",
                Description = "Pallof press hold for core anti-rotation stability",
                ExerciseType = "isolation",
                Equipment = "Cable Machine or Band",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/watch?v=b4bidbr5K78"
            },
            new()
            {
                Name = "Side Back Extension",
                Description = "Lateral back extension targeting obliques and lower back",
                ExerciseType = "isolation",
                Equipment = "Roman Chair or GHD",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=IqtzHGenpNU"
            },

            // ==================== LOWER BODY ====================
            // Glutes
            new()
            {
                Name = "Hip Thrust",
                Description = "Glute bridge with back on bench",
                ExerciseType = "compound",
                Equipment = "Barbell, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=5S8SApGU_Lk"
            },
            // Quadriceps
            new()
            {
                Name = "Squat",
                Description = "Barbell back squat",
                ExerciseType = "compound",
                Equipment = "Barbell, Squat Rack",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=my0tLDaWyDU"
            },
            new()
            {
                Name = "Front Squat",
                Description = "Barbell squat with bar in front rack position",
                ExerciseType = "compound",
                Equipment = "Barbell, Squat Rack",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=7pyxT5hqmQY"
            },
            new()
            {
                Name = "Bodyweight Squat",
                Description = "Air squat with no added weight",
                ExerciseType = "compound",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=0EpP3pYUYTk"
            },
            new()
            {
                Name = "Leg Press",
                Description = "Machine press for quadriceps and glutes",
                ExerciseType = "compound",
                Equipment = "Leg Press Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=q4W4_VJbKW0"
            },
            new()
            {
                Name = "Leg Extension",
                Description = "Machine extension for quadriceps",
                ExerciseType = "isolation",
                Equipment = "Leg Extension Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=4ZDm5EbiFI8"
            },
            new()
            {
                Name = "Lunges",
                Description = "Stepping lunge with bodyweight or weights",
                ExerciseType = "compound",
                Equipment = "Bodyweight or Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=wrwwXE_x-pQ"
            },
            new()
            {
                Name = "Bulgarian Split Squat",
                Description = "Single leg squat with rear foot elevated",
                ExerciseType = "compound",
                Equipment = "Dumbbells, Bench",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=Fmjj7wFJWRE"
            },
            // Hamstrings
            new()
            {
                Name = "Romanian Deadlift",
                Description = "Hip hinge movement targeting hamstrings",
                ExerciseType = "compound",
                Equipment = "Barbell",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=7j-2w4-P14I"
            },
            new()
            {
                Name = "Leg Curl",
                Description = "Machine curl for hamstrings",
                ExerciseType = "isolation",
                Equipment = "Leg Curl Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=Orxowest56U"
            },
            new()
            {
                Name = "Nordic Curl",
                Description = "Eccentric hamstring curl with bodyweight",
                ExerciseType = "isolation",
                Equipment = "Nordic Curl Station or Partner",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=_e9vFU9-tkc"
            },
            // Hip Flexors
            new()
            {
                Name = "Hip Flexor Raise",
                Description = "Raising the knee against gravity for hip flexor strength",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Ankle Weights",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=djxTlEIJdqc"
            },
            // Adductors
            new()
            {
                Name = "Hip Adductor",
                Description = "Machine adduction for inner thighs",
                ExerciseType = "isolation",
                Equipment = "Adductor Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=e9AqTFMmP18"
            },
            new()
            {
                Name = "Cossack Squat",
                Description = "Side-to-side squat for adductors and mobility",
                ExerciseType = "compound",
                Equipment = "Bodyweight or Dumbbells",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=iPZNB5GsOnM"
            },
            // Abductors
            new()
            {
                Name = "Hip Abductor",
                Description = "Machine abduction for outer hips",
                ExerciseType = "isolation",
                Equipment = "Abductor Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=G_8LItOiZ0Q"
            },
            // Calves
            new()
            {
                Name = "Calf Raise",
                Description = "Standing or seated calf raise",
                ExerciseType = "isolation",
                Equipment = "Machine or Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=c5Kv6-fnTj8"
            },
            new()
            {
                Name = "Reverse Calf Raise",
                Description = "Tibialis raise for shin muscles",
                ExerciseType = "isolation",
                Equipment = "Bodyweight or Tibialis Machine",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=o5OosBCxfU8"
            },

            // ==================== CARDIO ====================
            new()
            {
                Name = "Running",
                Description = "Outdoor or treadmill running",
                ExerciseType = "cardio",
                Equipment = "Treadmill or Outdoors",
                IsTimeBased = true,

            },
            new()
            {
                Name = "Cycling",
                Description = "Stationary bike or outdoor cycling",
                ExerciseType = "cardio",
                Equipment = "Bike",
                IsTimeBased = true,
            },
            new()
            {
                Name = "Rowing",
                Description = "Rowing machine cardio",
                ExerciseType = "cardio",
                Equipment = "Rowing Machine",
                IsTimeBased = true,
            },
            new()
            {
                Name = "Jump Rope",
                Description = "Skipping rope cardio",
                ExerciseType = "cardio",
                Equipment = "Jump Rope",
                IsTimeBased = true,
            },

            // ==================== PLYOMETRICS ====================
            new()
            {
                Name = "Box Jump",
                Description = "Explosive jump onto a box or platform",
                ExerciseType = "plyometric",
                Equipment = "Plyo Box",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=k7dmYdknbac"
            },
            new()
            {
                Name = "Depth Jump",
                Description = "Step off a box and immediately jump upon landing",
                ExerciseType = "plyometric",
                Equipment = "Plyo Box",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=AzPJZHOmGEg"
            },
            new()
            {
                Name = "Broad Jump",
                Description = "Explosive horizontal jump for distance",
                ExerciseType = "plyometric",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=uhz-ia-2UcM"
            },
            new()
            {
                Name = "Tuck Jump",
                Description = "Vertical jump bringing knees to chest at peak",
                ExerciseType = "plyometric",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=r7oBejx1PHM"
            },
            new()
            {
                Name = "Plyo Push-Up",
                Description = "Explosive push-up where hands leave the ground",
                ExerciseType = "plyometric",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=hDP-oskzYUs"
            },
            new()
            {
                Name = "Lateral Bound",
                Description = "Explosive side-to-side single-leg jumps",
                ExerciseType = "plyometric",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=soqQy4dzEts"
            },
            new()
            {
                Name = "Squat Jump",
                Description = "Bodyweight squat with explosive jump at the top",
                ExerciseType = "plyometric",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=BRfxI2Es2lE"
            },
            new()
            {
                Name = "Single-Leg Box Jump",
                Description = "Explosive single-leg jump onto a box",
                ExerciseType = "plyometric",
                Equipment = "Plyo Box",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=HVkjI6GY-OI"
            },

            // Boxing Plyometrics
            new()
            {
                Name = "Band Punch",
                Description = "Explosive punches against resistance band tension",
                ExerciseType = "plyometric",
                Equipment = "Resistance Band",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=eUSo-64Dt-Y"
            },
            new()
            {
                Name = "Medicine Ball Chest Pass",
                Description = "Explosive two-handed push throw from the chest for punching power",
                ExerciseType = "plyometric",
                Equipment = "Medicine Ball",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=V5Stj1U_Cfw"
            },
            new()
            {
                Name = "Medicine Ball Rotational Throw",
                Description = "Explosive rotational throw against a wall for hook and cross power",
                ExerciseType = "plyometric",
                Equipment = "Medicine Ball",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=o9BC7lgN1bo"
            },
            new()
            {
                Name = "Medicine Ball Slam",
                Description = "Overhead slam driving the ball into the ground with full body power",
                ExerciseType = "plyometric",
                Equipment = "Medicine Ball",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=QxYhFwMd1Ks"
            },
            new()
            {
                Name = "Medicine Ball Overhead Throw",
                Description = "Explosive backward overhead throw for hip extension and punching power",
                ExerciseType = "plyometric",
                Equipment = "Medicine Ball",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=u9-nFW_NyCU"
            },
            new()
            {
                Name = "Medicine Ball Scoop Toss",
                Description = "Underhand upward throw from a squat position for uppercut power",
                ExerciseType = "plyometric",
                Equipment = "Medicine Ball",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=mxf1D3NtxMo"
            },

            // ==================== MOBILITY ====================
            new()
            {
                Name = "Foam Rolling",
                Description = "Self-myofascial release with foam roller",
                ExerciseType = "mobility",
                Equipment = "Foam Roller",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/watch?v=DzSU2FiFKTM"
            },
            new()
            {
                Name = "Hip Flexor Stretch",
                Description = "Stretching the hip flexor muscles",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/shorts/VKnwWEDdT8w"
            },
            new()
            {
                Name = "Shoulder Dislocates",
                Description = "Shoulder mobility with band or stick",
                ExerciseType = "mobility",
                Equipment = "Band or Stick",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=SL8VAYpmpCQ"
            },
            new()
            {
                Name = "Pigeon Stretch",
                Description = "Hip opener stretch targeting glutes and hip rotators",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true,
                ExampleMedia = "https://www.youtube.com/shorts/pjmR5Kacu1w"
            },
            new()
            {
                Name = "Leg Swings",
                Description = "Dynamic leg swings for hip mobility warmup",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = false,
                ExampleMedia = "https://www.youtube.com/watch?v=difYoBtZi2s"
            },
            new()
            {
                Name = "Hip External Rotation Hold",
                Description = "Isometric hold in external hip rotation",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true,
            },
            new()
            {
                Name = "Hip Internal Rotation Hold",
                Description = "Isometric hold in internal hip rotation",
                ExerciseType = "mobility",
                Equipment = "Bodyweight",
                IsTimeBased = true,
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

        AddMapping("Decline Push-Up", "Upper Chest", "primary");
        AddMapping("Decline Push-Up", "Lower Chest", "secondary");
        AddMapping("Decline Push-Up", "Triceps", "secondary");
        AddMapping("Decline Push-Up", "Front Delts", "primary");

        AddMapping("Bulgarian Push-Up", "Upper Chest", "primary");
        AddMapping("Bulgarian Push-Up", "Lower Chest", "primary");
        AddMapping("Bulgarian Push-Up", "Triceps", "secondary");
        AddMapping("Bulgarian Push-Up", "Front Delts", "secondary");

        AddMapping("Decline Bulgarian Push-Up", "Upper Chest", "primary");
        AddMapping("Decline Bulgarian Push-Up", "Lower Chest", "secondary");
        AddMapping("Decline Bulgarian Push-Up", "Triceps", "secondary");
        AddMapping("Decline Bulgarian Push-Up", "Front Delts", "primary");

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

        AddMapping("Single Arm Cross Body Tricep Extension", "Triceps", "primary");

        // ==================== ARMS - FOREARMS ====================
        AddMapping("Wrist Rolls", "Forearms", "primary");
        AddMapping("Reverse Wrist Rolls", "Forearms", "primary");
        AddMapping("Hand Supination", "Forearms", "primary");
        AddMapping("Hand Pronation", "Forearms", "primary");
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

        // ==================== PLYOMETRICS ====================
        AddMapping("Box Jump", "Quadriceps", "primary");
        AddMapping("Box Jump", "Glutes", "primary");
        AddMapping("Box Jump", "Calves", "secondary");
        AddMapping("Box Jump", "Hamstrings", "secondary");

        AddMapping("Depth Jump", "Quadriceps", "primary");
        AddMapping("Depth Jump", "Glutes", "primary");
        AddMapping("Depth Jump", "Calves", "secondary");
        AddMapping("Depth Jump", "Hamstrings", "secondary");

        AddMapping("Broad Jump", "Quadriceps", "primary");
        AddMapping("Broad Jump", "Glutes", "primary");
        AddMapping("Broad Jump", "Hamstrings", "secondary");

        AddMapping("Tuck Jump", "Quadriceps", "primary");
        AddMapping("Tuck Jump", "Hip Flexors", "secondary");
        AddMapping("Tuck Jump", "Calves", "secondary");

        AddMapping("Plyo Push-Up", "Upper Chest", "primary");
        AddMapping("Plyo Push-Up", "Lower Chest", "primary");
        AddMapping("Plyo Push-Up", "Triceps", "secondary");
        AddMapping("Plyo Push-Up", "Front Delts", "secondary");

        AddMapping("Lateral Bound", "Glutes", "primary");
        AddMapping("Lateral Bound", "Abductors", "primary");
        AddMapping("Lateral Bound", "Quadriceps", "secondary");
        AddMapping("Lateral Bound", "Adductors", "secondary");

        AddMapping("Squat Jump", "Quadriceps", "primary");
        AddMapping("Squat Jump", "Glutes", "primary");
        AddMapping("Squat Jump", "Calves", "secondary");

        AddMapping("Single-Leg Box Jump", "Quadriceps", "primary");
        AddMapping("Single-Leg Box Jump", "Glutes", "primary");
        AddMapping("Single-Leg Box Jump", "Calves", "secondary");

        // Boxing Plyometrics
        AddMapping("Band Punch", "Upper Chest", "primary");
        AddMapping("Band Punch", "Front Delts", "primary");
        AddMapping("Band Punch", "Triceps", "secondary");

        AddMapping("Medicine Ball Chest Pass", "Upper Chest", "primary");
        AddMapping("Medicine Ball Chest Pass", "Lower Chest", "primary");
        AddMapping("Medicine Ball Chest Pass", "Triceps", "secondary");
        AddMapping("Medicine Ball Chest Pass", "Front Delts", "secondary");

        AddMapping("Medicine Ball Rotational Throw", "Obliques", "primary");
        AddMapping("Medicine Ball Rotational Throw", "Upper Chest", "secondary");
        AddMapping("Medicine Ball Rotational Throw", "Front Delts", "secondary");

        AddMapping("Medicine Ball Slam", "Abs", "primary");
        AddMapping("Medicine Ball Slam", "Lats", "primary");
        AddMapping("Medicine Ball Slam", "Front Delts", "secondary");

        AddMapping("Medicine Ball Overhead Throw", "Glutes", "primary");
        AddMapping("Medicine Ball Overhead Throw", "Hamstrings", "primary");
        AddMapping("Medicine Ball Overhead Throw", "Front Delts", "secondary");

        AddMapping("Medicine Ball Scoop Toss", "Quadriceps", "primary");
        AddMapping("Medicine Ball Scoop Toss", "Glutes", "primary");
        AddMapping("Medicine Ball Scoop Toss", "Front Delts", "secondary");

        await AppDatabase.Database.InsertAllAsync(exerciseMuscles);
    }
}
