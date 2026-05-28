Imports System.IO
Module Main
    Private output As String = ""

    Sub Main(args As String())
        If args.Length >= 3 Then
            Select Case args(0).ToLower
                Case "-f"          '--------> Encrypt file
                    If File.Exists(args(1)) Then
                        Dim fI As New FileInfo(args(1))
                        If AESEncryptFile(fI.FullName, args(2), args(3), args(4)) Then
                            Console.WriteLine("(i) Successfully encrypted" & vbCrLf)
                        Else
                            Console.WriteLine("(!) Could not encrypt" & vbCrLf)
                        End If
                    Else
                        Console.WriteLine("(!) File not found" & vbCrLf)
                    End If

                Case "-fd"          '--------> Decrypt file
                    If File.Exists(args(1)) Then
                        If AESDecryptFile(args(1), args(2), args(3), args(4)) Then
                            Console.WriteLine("(i) Successfully decrypted" & vbCrLf)
                        Else
                            Console.WriteLine("(!) Could not decrypt" & vbCrLf)
                        End If
                    Else
                        Console.WriteLine("(!) File not found")
                    End If

                Case "-s"          '--------> Encrypt string
                    output = Encrypt(args(1), args(2) + args(3))
                    Console.WriteLine(output)

                Case "-sd"          '--------> Decrypt string
                    output = Decrypt(args(1), args(2) + args(3))
                    Console.WriteLine(output)

                Case "-d"          '--------> Encrypt whole dir
                    If Directory.Exists(args(1)) Then
                        For Each file As String In Directory.GetFiles(args(1))
                            Dim finfo As New FileInfo(file)
                            Console.WriteLine("> Encrypting " + file & vbCrLf)
                            If AESEncryptFile(file, Path.Combine(args(2), finfo.Name), args(3), args(4)) Then
                                Console.WriteLine("(i) Successfully encrypted " + file & vbCrLf)
                            Else
                                Console.WriteLine("(!) Could not encrypt " + file & vbCrLf)
                            End If
                        Next
                        Console.WriteLine("(i) Done")
                    Else
                        Console.WriteLine("(!) Directory " + args(0) + " Not found" & vbCrLf)
                    End If

                Case "-dd"          '--------> Decrypt whole dir
                    If Directory.Exists(args(1)) Then
                        For Each file As String In Directory.GetFiles(args(1))
                            Dim finfo As New FileInfo(file)
                            Console.WriteLine("> Decrypting " + file & vbCrLf)
                            If AESDecryptFile(file, Path.Combine(args(2), finfo.Name), args(3), args(4)) Then
                                Console.WriteLine("(i) Successfully decrypted " + file & vbCrLf)
                            Else
                                Console.WriteLine("(!) Could not decrypt " + file & vbCrLf)
                            End If
                        Next
                        Console.WriteLine("(i) Done")
                    Else
                        Console.WriteLine("(!) Directory " + args(0) + " Not found")
                    End If
            End Select
        Else
            DisplayHelp()
        End If
    End Sub
    Sub DisplayHelp()
        Console.WriteLine("CMDCrypt - Encrypt files and strings with AES CBC" & vbCrLf + vbCrLf)
        Console.WriteLine("(c) Luca Franziskowski, All rights reserved. Build in 2022" & vbCrLf)
        Console.WriteLine("Parameters:")
        Console.WriteLine("-f  : Encrypt a file   -s  : Encrypt a string     -d  : Encrypt whole directory")
        Console.WriteLine("-fd : Decrypt a file   -sd : Decrypt a string     -dd : Decrypt whole directory" & vbCrLf)
        Console.WriteLine("-> Please note: if you are encrypting/decrypting a whole directory, " & vbCrLf +
                          "you need to choose a source- and destination-directory." & vbCrLf +
                          "-> Your salt needs to be at least 8 Bytes!")
        Console.WriteLine("Syntax:")
        Console.WriteLine("cmdCrypt.exe <Parameter> ""Source-Filename"" ""Output-Filename"" ""Password"" ""Salt""" & vbCrLf)
        Console.WriteLine("Example:")
        Console.WriteLine("cmdCrypt.exe -f ""C:\Users\myFile.txt"" ""D:\Example\encrypted.txt"" abcde123fg IamASalt")
        Console.WriteLine("cmdCrypt.exe -dd ""C:\MySourceFolder"" ""D:\MyDestinationFolder"" abcde123fg Thisis1asalt")
        Console.Read()
    End Sub

End Module
