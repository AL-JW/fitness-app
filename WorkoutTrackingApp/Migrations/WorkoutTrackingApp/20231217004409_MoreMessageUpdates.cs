using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTrackingApp.Migrations.WorkoutTrackingApp
{
    /// <inheritdoc />
    public partial class MoreMessageUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedExerciseExerciseId",
                table: "Messages",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedWorkoutWorkoutId",
                table: "Messages",
                type: "int",
                nullable: true,
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
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Workouts_SelectedWorkoutWorkoutId",
                table: "Messages",
                column: "SelectedWorkoutWorkoutId",
                principalTable: "Workouts",
                principalColumn: "WorkoutId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
