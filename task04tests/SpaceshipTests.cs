using Xunit;
using task04;
using System;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            ISpaceship fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(50, fighter.FirePower);
        }

        [Fact]
        public void Cruiser_Fire_ShouldBeImplemented()
        {
            var cruiser = new Cruiser();
            
            // Act & Assert (проверяем, что метод не бросает исключение)
            var exception = Record.Exception(() => cruiser.Fire());
            Assert.Null(exception);
        }

        [Fact]
        public void Fighter_Rotate_ShouldBeImplemented()
        {
            var fighter = new Fighter();
            
            // Act & Assert (проверяем, что метод не бросает исключение)
            var exception = Record.Exception(() => fighter.Rotate(45));
            Assert.Null(exception);
        }

        [Fact]
        public void MoveForward_ShouldBeImplemented()
        {
            var fighter = new Fighter();
            
            // Act & Assert (проверяем, что метод не бросает исключение)
            var exception = Record.Exception(() => fighter.MoveForward());
            Assert.Null(exception);
        }

        [Fact]
        public void Cruiser_FirePower_ShouldBeGreaterThanFighter()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(cruiser.FirePower > fighter.FirePower);
        }

        [Fact]
        public void Rotate_WithNegativeAngle_ShouldBeHandled()
        {
            var cruiser = new Cruiser();
            
            // Act & Assert (проверяем, что метод не бросает исключение)
            var exception = Record.Exception(() => cruiser.Rotate(-90));
            Assert.Null(exception);
        }

        [Fact]
        public void MoveForward_ShouldOutputCorrectMessage()
        {
            var cruiser = new Cruiser();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            cruiser.MoveForward();
            Assert.Contains("Крейсер движется вперед", consoleOutput.ToString());
        }
    }
}