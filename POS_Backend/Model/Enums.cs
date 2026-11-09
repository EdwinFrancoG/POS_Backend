namespace POS_Backend.Model;

public enum AuditAction { Create, Update, Delete, Login, Logout, Void }
public enum PaymentTerm { Contado, Credito }
public enum InvoiceStatus { Borrador, Emitida, Pagada, Anulada }
public enum PaymentMethod { Efectivo, Tarjeta, Transferencia, Cheque }
public enum CashClosingStatus { Abierto, Cerrado, Anulado }