namespace GreenT.Settings.Data;

public enum PostRequestType
{
	EmailCheck = 0,
	Authorization = 1,
	SetUserData = 2,
	GetUserData = 3,
	GetTime = 5,
	PaymentCheckout = 8,
	PaymentIntent = 9,
	ReceivedPayment = 10,
	PaymentStatusUpdate = 19,
	PlayerInvoiceStatus = 15,
	SuspendedPayments = 16,
	PlayerCheck = 11,
	Registration = 12,
	Login = 13,
	WebGLReader = 14,
	PartnerEvent = 7,
	AmplitudeEventServerMirror = 17,
	ConfigVersion = 18,
	SteamPurchaseConfirm = 20,
	GetLeaderboard = 21,
	AddLeaderboardPoints = 22,
	LeaderboardRegistration = 23,
	SubscriptionsActive = 26,
	UserUpdate = 30,
	GetUserDataByToken = 31,
	CheaterRequest = 32,
	UserInfo = 33,
	ErolabsGetBalanceRequest = 34,
	Promocodes = 35
}
