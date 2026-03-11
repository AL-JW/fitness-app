using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTrackingApp.Migrations.WorkoutTrackingApp
{
    /// <inheritdoc />
    public partial class UpdatedingMessagesToIncludeWorkoutsAndSpecificExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Exercises_SelectedExerciseExerciseId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Workouts_SelectedWorkoutWorkoutId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SelectedExerciseExerciseId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SelectedWorkoutWorkoutId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SelectedExerciseExerciseId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SelectedWorkoutWorkoutId",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "SelectedExerciseId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedWorkoutId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SelectedExerciseId",
                table: "Messages",
                column: "SelectedExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SelectedWorkoutId",
                table: "Messages",
                column: "SelectedWorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Exercises_SelectedExerciseId",
                table: "Messages",
                column: "SelectedExerciseId",
                principalTable: "Exercises",
                principalColumn: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Workouts_SelectedWorkoutId",
                table: "Messages",
                column: "SelectedWorkoutId",
                principalTable: "Workouts",
                principalColumn: "WorkoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Exercises_SelectedExerciseId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Workouts_SelectedWorkoutId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SelectedExerciseId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SelectedWorkoutId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SelectedExerciseId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SelectedWorkoutId",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "SelectedExerciseExerciseId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedWorkoutWorkoutId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SelectedExerciseExerciseId",
                table: "Messages",
                column: "SelectedExerciseExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SelectedWorkoutWorkoutId",
                table: "Messages",
                column: "SelectedWorkoutWorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Exercises_SelectedExerciseExerciseId",
                table: "Messages",
                column: "SelectedExerciseExerciseId",
                principalTable: "Exercises",
                principalColumn: "ExerciseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Workouts_SelectedWorkoutWorkoutId",
                table: "Messages",
                column: "SelectedWorkoutWorkoutId",
                principalTable: "Workouts",
                principalColumn: "WorkoutId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
