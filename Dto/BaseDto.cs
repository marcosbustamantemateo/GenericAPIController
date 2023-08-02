namespace GenericControllerLib.Dto
{
	/// <summary>
	///     Objeto a devolver en las respuestas de las APIs
	/// </summary>
	/// <param name="Message">Mensaje informativo</param>
	/// <param name="data">Dato a devolver</param>
	public record struct BaseDto(string Message, object? data);
}
