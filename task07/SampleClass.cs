using System;

namespace task07
{
    [Version(1, 0)]
    [DisplayName("Пример класса")]
    public class SampleClass
    {
        [DisplayName("Тестовый метод")]
        public void TestMethod() { }

        [DisplayName("Числовое свойство")]
        public int Number { get; set; } = 42;

        public string UserName { get; set; } = "John";
        public void HiddenMethod() { }
    }
}