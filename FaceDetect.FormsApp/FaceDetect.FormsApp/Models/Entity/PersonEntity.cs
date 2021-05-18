namespace FaceDetect.FormsApp.Models.Entity
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Smart.Data.Mapper.Attributes;

    public class PersonEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [AllowNull]
        public string Name { get; set; }
    }
}
