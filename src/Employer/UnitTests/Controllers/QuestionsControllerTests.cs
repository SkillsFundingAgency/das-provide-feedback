using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ESFA.DAS.EmployerProvideFeedback.Configuration.Routing;
using ESFA.DAS.EmployerProvideFeedback.Controllers;
using ESFA.DAS.EmployerProvideFeedback.Infrastructure;
using ESFA.DAS.EmployerProvideFeedback.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace UnitTests.Controllers
{
    public class QuestionsControllerTests
    {
        private QuestionsController _controller;
        private Mock<ISessionService> _sessionServiceMock;
        private Mock<IOptions<List<ProviderSkill>>> _provSKillsOptions;
        private IFixture _fixture;
        private List<ProviderSkill> _providerSkills;
        private Guid _uniqueCode = Guid.NewGuid();

        public QuestionsControllerTests()
        {
            _fixture = new Fixture();
            _sessionServiceMock = new Mock<ISessionService>();
            _provSKillsOptions = new Mock<IOptions<List<ProviderSkill>>>();

            _providerSkills = GetProviderSkills();
            _provSKillsOptions.SetupGet(mock => mock.Value).Returns(_providerSkills);
            _controller = new QuestionsController(_sessionServiceMock.Object, _provSKillsOptions.Object);
        }

        [Fact]
        public void Question_1_When_No_Session_Answers_Should_Have_No_Doing_Well_Attributes()
        {
            // Arrange

            // Act
            var result = _controller.QuestionOne(_uniqueCode) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<List<ProviderSkill>>(result.Model);
            var model = result.Model as List<ProviderSkill>;
            Assert.DoesNotContain(model, m => m.IsDoingWell);
        }

        [Fact]
        public void Question_1_When_Session_Answers_Should_Mark_As_Doing_Well()
        {
            // Arrange
            var answerModel = new AnswerModel();
            var sessionDoingWellSkills = _providerSkills.Take(3).ToList();
            sessionDoingWellSkills.ForEach(ps => ps.IsDoingWell = true);
            answerModel.ProviderSkills = _providerSkills;
            _sessionServiceMock.Setup(mock => mock.Get<AnswerModel>(It.IsAny<string>())).Returns(answerModel);

            // Act
            var result = _controller.QuestionOne(_uniqueCode) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<List<ProviderSkill>>(result.Model);
            var model = result.Model as List<ProviderSkill>;
            Assert.Contains(model, m => m.IsDoingWell);
            Assert.Equal(sessionDoingWellSkills.Count, model.Count(m => m.IsDoingWell));
        }

        [Fact]
        public void Question_1_When_Answers_Submitted_Should_Update_Session_And_Redirect()
        {
            // Arrange
            var sessionDoingWellSkills = _providerSkills.Take(3).ToList();
            sessionDoingWellSkills.ForEach(ps => ps.IsDoingWell = true);

            // Act
            var result = _controller.QuestionOne(_uniqueCode, _providerSkills);

            // Assert
            _sessionServiceMock.Verify(mock => mock.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            Assert.Equal(RouteNames.QuestionTwo_Get, (result as RedirectToRouteResult).RouteName);
        }

        [Fact]
        public void Question_2_When_Q1_Skipped_Should_Have_No_Skills_Doing_Well()
        {
            // Arrange

            // Act
            var result = _controller.QuestionTwo(_uniqueCode) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<List<ProviderSkill>>(result.Model);
            var model = result.Model as List<ProviderSkill>;
            Assert.DoesNotContain(model, m => m.IsDoingWell);
        }

        [Fact]
        public void Question_2_When_Q1_Skipped_And_Q2_Session_Answers_Should_Load_Previous_Selections()
        {
            // Arrange
            var answerModel = new AnswerModel();
            var sessionDoingWellSkills = _providerSkills.Take(3).ToList();
            sessionDoingWellSkills.ForEach(ps => ps.IsToImprove = true);
            answerModel.ProviderSkills = _providerSkills;
            _sessionServiceMock.Setup(mock => mock.Get<AnswerModel>(It.IsAny<string>())).Returns(answerModel);

            // Act
            var result = _controller.QuestionTwo(_uniqueCode) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<List<ProviderSkill>>(result.Model);
            var model = result.Model as List<ProviderSkill>;
            Assert.Contains(model, m => m.IsToImprove);
            Assert.Equal(sessionDoingWellSkills.Count, model.Count(m => m.IsToImprove));
        }

        [Fact]
        public void Question_2_When_Answers_Submitted_Should_Update_Session_And_Redirect()
        {
            // Arrange
            var sessionDoingWellSkills = _providerSkills.Take(3).ToList();
            sessionDoingWellSkills.ForEach(ps => ps.IsToImprove = true);

            // Act
            var result = _controller.QuestionTwo(_uniqueCode, _providerSkills);

            // Assert
            _sessionServiceMock.Verify(mock => mock.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            Assert.Equal(RouteNames.QuestionThree_Get, (result as RedirectToRouteResult).RouteName);
        }

        [Fact]
        public void Question_3_When_Q1_And_Q2_Skipped_Should_Have_No_Selected_Skills()
        {
            // Arrange

            // Act
            var result = _controller.QuestionThree(_uniqueCode) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<AnswerModel>(result.Model);
            var model = result.Model as AnswerModel;
            Assert.False(model.HasStrengths);
            Assert.False(model.HasWeaknesses);
        }

        [Fact]
        public void Question_3_When_Q1_And_Q2_Skipped_And_Q3_Session_Answers_Should_Load_Previous_Selection()
        {
            // Arrange
            var answerModel = new AnswerModel();
            answerModel.ProviderRating = ProviderRating.Poor;
            _sessionServiceMock.Setup(mock => mock.Get<AnswerModel>(It.IsAny<string>())).Returns(answerModel);

            // Act
            var result = _controller.QuestionThree(_uniqueCode) as ViewResult;

            // Assert
            Assert.IsAssignableFrom<AnswerModel>(result.Model);
            var model = result.Model as AnswerModel;
            Assert.Equal(ProviderRating.Poor, model.ProviderRating);
        }

        [Fact]
        public void Question_3_When_Answer_Not_Selected_Should_Fail_Model_Validation()
        {
            // Arrange
            var answerModel = new AnswerModel();
            _sessionServiceMock.Setup(mock => mock.Get<AnswerModel>(It.IsAny<string>())).Verifiable();

            // simulate model validation as this only occurs at runtime
            _controller.ModelState.AddModelError("ProviderRating", "Required Field");

            // Act
            var result = _controller.QuestionThree(_uniqueCode, answerModel);

            // Assert
            _sessionServiceMock.Verify(mock => mock.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
            Assert.IsAssignableFrom<ViewResult>(result);
        }

        [Fact]
        public void Question_3_When_Answers_Submitted_Should_Update_Session_And_Redirect()
        {
            // Arrange
            var answerModel = new AnswerModel { ProviderRating = ProviderRating.Excellent };

            // Act
            var result = _controller.QuestionThree(_uniqueCode, answerModel);

            // Assert
            _sessionServiceMock.Verify(mock => mock.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            var redirectResult = Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            Assert.Equal(RouteNames.ReviewAnswers_Get, redirectResult.RouteName);
        }

        private List<ProviderSkill> GetProviderSkills()
        {
            return _fixture
                .Build<ProviderSkill>()
                .With(x => x.IsDoingWell, false)
                .With(x => x.IsToImprove, false)
                .CreateMany(10)
                .ToList();
        }
    }
}