namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed => 50;     
        public int FirePower => 100;

        public void MoveForward()
        {
            Console.WriteLine("Крейсер движется вперед");
        }

        public void Rotate(int angle)
        {
            Console.WriteLine($"Крейсер поворачивает на {angle}°");
        }

        public void Fire()
        {
            Console.WriteLine("Крейсер производит выстрел");
        }
    }
}