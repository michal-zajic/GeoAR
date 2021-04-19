using System.Collections.Generic;
using System.Runtime.InteropServices;

//allows invoking haptic feedback on ios devices, currently not used, however it may be useful in future
public class IOSNative 
{
	[DllImport("__Internal")]
	private static extern void HapticFeedback (int type);

	public static void StartHapticFeedback(HapticFeedbackTypes type)
	{
		HapticFeedback ((int)type);
	}
}

public enum HapticFeedbackTypes
{
    LIGHT,
    MEDIUM,
    HEAVY
}
