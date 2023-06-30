
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlogEngine.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Body { get; set; }
    
    [Required]
    public DateTime DatePublished {get; set;}

    [Required]
    public DateTime DateModified {get; set;}

    public string Tags {get; set;}
    
    public string AuthorId {get; set;}
    public Author? Author {get; set;}
    
}
