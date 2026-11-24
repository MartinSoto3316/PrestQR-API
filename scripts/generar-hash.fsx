// Genera el hash BCrypt de una contraseña para guardarlo en usuarios.password_hash.
// Uso: dotnet fsi generar-hash.fsx "TuContraseña"
#r "nuget: BCrypt.Net-Next, 4.2.0"

match fsi.CommandLineArgs |> Array.tail with
| [| password |] -> printfn "%s" (BCrypt.Net.BCrypt.HashPassword password)
| _ -> eprintfn "Uso: dotnet fsi generar-hash.fsx \"TuContraseña\""
