using System.ComponentModel;

namespace CustomServiceTestUtil
{
    public enum NoYes : int
    {
        No = 0,
        Yes = 1
    }
    public enum Direction : int
    {
        [Description("No validation")]
        None = 0,
        [Description("Call")]
        Call = 1,
        [Description("Response")]
        Response = 2,
    }
    public enum CallAction
    {
        [Description("No parameters")]
        No = 0,
        [Description("Parameters")]
        Yes = 1,
        [Description("File")]
        File = 2

    }
    public enum SaveAction
    {
        Response,
        Validation
    }
}
