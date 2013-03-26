using System;
using System.Collections;

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

        static void Chip8SoundGenerated()
        {
            Console.Beep();
        }

        // Actualizar pantalla de acuerdo a los valores del arreglo.
        // En esta implementación se escribe en la consola del sistema
        static void Chip8ScreenChanged(BitArray graphics)
        {
            // Limpiamos la pantalla, también se puede usar Console.Clear();
            Console.Clear();

            // Pintamos bordes superiores
            Console.WriteLine("╔" + "".PadRight(C8Constants.ResolutionWidth, '═') + "╗");

            // Se pinta la pantalla
            for (int y = 0; y < C8Constants.ResolutionHeight; y++)
            {
                Console.Write("║");	 // Usamos un pipe (|) para los bordes de pantalla
                
                for (var x = 0; x < C8Constants.ResolutionWidth; x++)
                {
                    Console.Write(GetPixelState(graphics, x,y) ? "█" : " ");
                }

                Console.WriteLine("║");
            }

            // Pintamos bordes inferiores
            Console.WriteLine("╚" + "".PadRight(C8Constants.ResolutionWidth, '═') + "╝");
            Console.WriteLine("");
        }

        /// <summary>
        /// Gets the state of a pixel, take into account that
        /// screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool GetPixelState(BitArray graphics, int x, int y)
        {
            return graphics[x + (C8Constants.ResolutionWidth * y)];
        }
    }
}
