namespace MinimalAPI.Domain.ModelViews;

public struct Home
{
    public string Message { get => 
    "Welcome to the Vehicle Management API/Bem-vindo a API Gerenciamento de Veículos"; }
    public string Documentation { get => "/swagger/index.html"; }
}