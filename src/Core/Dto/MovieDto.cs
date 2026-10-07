namespace Core.Dto;

public record MovieDto(
    string Id,
    string Title,
    string Genre,
    int Year,
    int Duration);