using System;

namespace C8POC.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var chip8 = new C8Engine();
            chip8.ScreenChanged += Chip8ScreenChanged;
            chip8.SoundGenerated += Chip8SoundGenerated;
            chip8.LoadEmulator(args[0]);
            chip8.StartEmulator();
        }

        static void Chip8SoundGenerated(object sender, EventArgs e)
        {
            Console.Beep();
        }

        // Actualizar pantalla de acuerdo a los valores del arreglo.
        // En esta implementación se escribe en la consola del sistema
        static void Chip8ScreenChanged(object sender, EventArgs e)
        {
            // Limpiamos la pantalla, también se puede usar Console.Clear();
            Console.Clear();

            // Pintamos bordes superiores
            Console.WriteLine("╔" + "".PadRight(64, '═') + "╗");

            // Se pinta la pantalla
            for (int y = 0; y < 32; y++)
            {
                Console.Write("║");	 // Usamos un pipe (|) para los bordes de pantalla
                
                for (var x = 0; x < 64; x++)
                {
                    Console.Write(((C8Engine) sender).GetPixelState(x, y) ? "█" : " ");
                }

                Console.WriteLine("║");
            }

            // Pintamos bordes inferiores
            Console.WriteLine("╚" + "".PadRight(64, '═') + "╝");
            Console.WriteLine("");
        }
    }
}
