namespace Application.Activities
{
    public class Result <T>
    {
        public bool IsSucess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }
        public static Result<T> Sucess(T value) => new Result<T> {IsSucess=true,Value =value};
        public static Result<T> Failure (string errror) => new Result<T> {IsSucess=false, Error= errror};
        
    }
}