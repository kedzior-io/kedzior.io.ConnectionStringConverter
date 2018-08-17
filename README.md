# kedzior.io.ConnectionStringConverter

Kedzior.io.ConnectionStringConverter is a library that simply converts non-standard MySQL connection string to standard one. Useful when using MySQL with asp.net core apps on Azure.

[![Build status](https://ci.appveyor.com/api/github/webhook?id=t9a89lywnuu1sj1r/branch/master?svg=true)](https://ci.appveyor.com/project/kedzior-io/kedzior-io-connectionstringconverter/branch/master)

## Install

Package Manager:
```powershell
Install-Package kedzior.io.ConnectionStringConverter
```

.NET CLI
```powershell
dotnet add package kedzior.io.ConnectionStringConverter
```

## Usage

Use it in `Startup.cs`

```csharp
	string connectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb");
		services.AddDbContext<ApplicationDbContext>(options =>  
			options.UseMySql(connectionString.ToMySQLStandard())
		);
```
