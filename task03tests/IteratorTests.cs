using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task03;

namespace task03tests
{
    public class IteratorTests
    {
        [Fact]
        public void CustomCollection_GetEnumerator_ReturnsAllItems()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);

            // Act
            var result = new List<int>();
            foreach (var item in collection)
            {
                result.Add(item);
            }

            // Assert
            Assert.Equal(new[] { 1, 2 }, result);
        }

        [Fact]
        public void GetReverseEnumerator_ReturnsItemsInReverseOrder()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);

            // Act
            var result = collection.GetReverseEnumerator().ToList();

            // Assert
            Assert.Equal(new[] { 3, 2, 1 }, result);
        }

        [Fact]
        public void GenerateSequence_ReturnsCorrectSequence()
        {
            // Act
            var sequence = CustomCollection<int>.GenerateSequence(5, 3).ToList();

            // Assert
            Assert.Equal(new[] { 5, 6, 7 }, sequence);
        }

        [Fact]
        public void FilterAndSort_ReturnsFilteredAndSortedItems()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            collection.Add(3);
            collection.Add(1);
            collection.Add(4);
            collection.Add(2);
            collection.Add(5);

            // Act
            var result = collection.FilterAndSort(
                x => x > 2,         // Фильтр: значения больше 2
                x => x               // Сортировка по возрастанию
            ).ToList();

            // Assert
            Assert.Equal(new[] { 3, 4, 5 }, result);
        }

        [Fact]
        public void GetReverseEnumerator_WithEmptyCollection_ReturnsEmpty()
        {
            // Arrange
            var collection = new CustomCollection<string>();

            // Act
            var result = collection.GetReverseEnumerator().ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GenerateSequence_WithZeroCount_ReturnsEmpty()
        {
            // Act
            var sequence = CustomCollection<int>.GenerateSequence(10, 0).ToList();

            // Assert
            Assert.Empty(sequence);
        }

        [Fact]
        public void FilterAndSort_WithCustomObjects()
        {
            // Arrange
            var collection = new CustomCollection<Person>();
            collection.Add(new Person("Alice", 30));
            collection.Add(new Person("Bob", 25));
            collection.Add(new Person("Charlie", 35));
            
            // Act: Фильтр по возрасту > 25 и сортировка по имени
            var result = collection.FilterAndSort(
                p => p.Age > 25,
                p => p.Name
            ).ToList();

            // Assert
            Assert.Collection(result,
                item => Assert.Equal("Alice", item.Name),
                item => Assert.Equal("Charlie", item.Name)
            );
        }

        [Fact]
        public void FilterAndSort_WithNullPredicate_ThrowsException()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                collection.FilterAndSort(null, x => x).ToList());
        }

        [Fact]
        public void FilterAndSort_WithNullKeySelector_ThrowsException()
        {
            // Arrange
            var collection = new CustomCollection<int>();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                collection.FilterAndSort(x => true, null).ToList());
        }

        private class Person
        {
            public string Name { get; }
            public int Age { get; }
            
            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }
        }
    }
}