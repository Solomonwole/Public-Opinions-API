namespace Crud.Api.DTOs
{
    public class OpinionDTOs
    {
        public record CreateOpinionDto(string Title, string Content);
        public record UpdateOpinionDto(string Title, string Content);
    }
}
