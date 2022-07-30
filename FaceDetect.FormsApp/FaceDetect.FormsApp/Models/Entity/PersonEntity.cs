namespace FaceDetect.FormsApp.Models.Entity;

using System;

using Smart.Data.Mapper.Attributes;

public class PersonEntity
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
}
