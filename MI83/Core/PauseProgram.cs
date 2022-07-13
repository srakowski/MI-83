namespace MI83.Core;

using Microsoft.Xna.Framework.Input;

class PauseProgram : IProgram
{
    public void ExecuteNextInstruction(Computer computer)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            computer.ExitPrgm();
        }
    }
}

