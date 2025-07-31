namespace ApplicationCore.Features.DevApps
{
    public class DevAppViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }

    public static class DevAppViewModelExtension
    {
        /// <summary>
        /// Deep Copy of Object
        /// </summary>
        /// <param name="value">Actual object to be copy</param>
        /// <returns>new instance of object</returns>
        public static DevAppViewModel Copy(this DevAppViewModel value)
        {
            if (value is null)
            {
                return new();
            }

            return new()
            {
                Id = value.Id,
                Name = value.Name,
                Path = value.Path,
            };
        }
    }
}
