namespace MI83.Core;

static class SysCommands
{
	public static void GetPrgms() {}
	public static void Menu() {}
	public static void RunPrgm() {}
	public static void EditPrgm() {}
	public static void Input() {}
	public static void CreatePrgm() {}

	public static int GetKey(this Computer cmp)
    {
		return cmp.InputBuffer.GetLastKeyUp();
    }

	public static void Pause(this Computer cmp) 
	{
		cmp.RunProgram("PAUSE");
	}
	
	public static void Disp(this Computer cmp, string value) 
	{
		cmp.DisplayMode = DisplayMode.Home;
		cmp.HomeBuffer.Disp(value, (byte)5, (byte)0);
	}

	public static void ClrHome() {}
	public static void GetSuppDispRes() {}
	public static void SetDispRes() {}
	
	public static void ExitPrgm(this Computer cmp) 
	{
		cmp.ExitProgram();
	}
}