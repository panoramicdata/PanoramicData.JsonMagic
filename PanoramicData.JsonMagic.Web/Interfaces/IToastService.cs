namespace PanoramicData.JsonMagic.Web.Interfaces;

public interface IToastService
{
	void Info(
		string message,
		string? title = null,
		bool allowHtml = false,
		ToastServiceCloseBehaviour closeBehaviour = ToastServiceCloseBehaviour.CloseAutomatically);

	void Success(
		string message,
		string? title = null,
		bool allowHtml = false,
		ToastServiceCloseBehaviour closeBehaviour = ToastServiceCloseBehaviour.CloseAutomatically);

	void Warning(
		string message,
		string? title = null,
		bool allowHtml = false,
		ToastServiceCloseBehaviour closeBehaviour = ToastServiceCloseBehaviour.CloseAutomatically);

	void Error(
		string message,
		string? title = null,
		bool allowHtml = false,
		ToastServiceCloseBehaviour closeBehaviour = ToastServiceCloseBehaviour.CloseAutomatically);
}