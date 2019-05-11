namespace CustomServiceTestUtil
{
    interface IArgumentParameters<T>
    {
        void SaveParameters(T serviceapi);
    }
    interface IOutputMessages<T>
    {
        void SetOutput(T output);
        void SetResponse(T response);
    }
}
