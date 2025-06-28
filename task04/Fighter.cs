namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100; 
        public int FirePower => 50; 

        public void MoveForward()
        {
            Console.WriteLine("Истребитель движется вперед");
        }

        public void Rotate(int angle)
        {
            Console.WriteLine($"Истребитель поворачивает на {angle}°");
        }

        public void Fire()
        {
            Console.WriteLine("Истребитель производит выстрел");
        }
    }
}