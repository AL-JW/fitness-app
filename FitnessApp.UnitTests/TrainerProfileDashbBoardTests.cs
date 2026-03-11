//using Xunit;
//using Microsoft.AspNetCore.Mvc;
//using WorkoutTrackingApp.Controllers;
//using WorkoutTrackingApp.ViewModels;
//using System.Collections.Generic;

//namespace WorkoutTrackingApp.Tests
//{
//    public class TrainerProfileControllerTests
//    {
//        [Fact]
//        public void Subscribers_ReturnsTheCorrectViewModel()
//        {
//            // Arrange
//            var controller = new TrainerProfileDashboardController();

//            // Act
//            var result = controller.Subscribers();

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var model = Assert.IsType<List<SubscriberViewModel>>(viewResult.Model);
//        }
//    }
//}
