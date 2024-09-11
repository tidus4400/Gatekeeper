namespace Gatekeeper.KeyLib.Models;

public abstract record GkpResult<TValue, TError>;

public record GkpOk<TValue, TError>(TValue Value) : GkpResult<TValue, TError>;

public record GkpErr<TValue, TError>(TError Error) : GkpResult<TValue, TError>;