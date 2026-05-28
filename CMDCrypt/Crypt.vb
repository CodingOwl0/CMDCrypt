Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Module Crypt

    Public Function Encrypt(input As String, key As String) As String
        Dim AES As New RijndaelManaged
        Dim SHA256 As New SHA256Cng
        Dim ciphertext As String
        Try
            AES.GenerateIV()
            AES.Key = SHA256.ComputeHash(Encoding.ASCII.GetBytes(key))
            AES.Mode = CipherMode.CBC
            Dim DESEncrypter As ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = Encoding.ASCII.GetBytes(input)
            ciphertext = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

            Return Convert.ToBase64String(AES.IV) & Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

        Catch ex As Exception
            Console.WriteLine("[1004] Could not encrypt string " + ex.ToString & vbCrLf)
        End Try
        Return ""
    End Function



    Public Function Decrypt(ciphertext As String, key As String) As String
        Dim AES As New RijndaelManaged
        Dim SHA256 As New SHA256Cng
        Dim plaintext As String
        Dim iv As String
        Try
            Dim ivct = ciphertext.Split({"=="}, StringSplitOptions.None)
            iv = ivct(0) & "=="
            ciphertext = If(ivct.Length = 3, ivct(1) & "==", ivct(1))

            AES.Key = SHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key))
            AES.IV = Convert.FromBase64String(iv)
            AES.Mode = CipherMode.CBC
            Dim DESDecrypter As ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = Convert.FromBase64String(ciphertext)
            plaintext = ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return plaintext
        Catch ex As Exception
            Console.WriteLine("[1005] Could not decrypt message- Maybe the key is invalid." + ex.ToString & vbCrLf)
        End Try
        Return ""
    End Function





    Public Function AESEncryptFile(plainFilePath As String, encryptedFilePath As String, EncryptionKey As String, SaltValue As String) As Boolean ', IV As String
        Try
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D,
                     &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            'Dim Key = pdb.GetBytes(32)
            'IV = pdb.GetBytes(16).ToString
            Dim initVectorBytes As Byte() = pdb.GetBytes(16) 'Encoding.ASCII.GetBytes(IV)
            Dim saltValueBytes As Byte() = Encoding.ASCII.GetBytes(SaltValue)
            Dim k1 As New Rfc2898DeriveBytes(EncryptionKey, saltValueBytes, 100)
            Dim symmetricKey As New AesManaged With {
                    .KeySize = 256,
                    .Mode = CipherMode.CBC
                }
            Dim encryptor As ICryptoTransform = symmetricKey.CreateEncryptor(k1.GetBytes(16), initVectorBytes)
            Using plain As FileStream = File.Open(plainFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Using encrypted As FileStream = File.Open(encryptedFilePath, FileMode.Create, FileAccess.Write, FileShare.None)
                    Using cs As New CryptoStream(encrypted, encryptor, CryptoStreamMode.Write)
                        plain.CopyTo(cs)
                    End Using
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine("[1006] Could not encrypt file: " + ex.ToString & vbCrLf)
        End Try
        Return False
    End Function

    Public Function AESDecryptFile(encryptedFilePath As String, plainFilePath As String, DecryptionKey As String, SaltValue As String) As Boolean ', IV As String
        Try
            Dim pdb As New Rfc2898DeriveBytes(DecryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D,
                     &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            Dim initVectorBytes As Byte() = pdb.GetBytes(16)  'Encoding.ASCII.GetBytes(IV)
            Dim saltValueBytes As Byte() = Encoding.ASCII.GetBytes(SaltValue)
            Dim k1 As New Rfc2898DeriveBytes(DecryptionKey, saltValueBytes, 100)
            Dim symmetricKey As New AesManaged With {
                    .KeySize = 256,
                    .Mode = CipherMode.CBC
                }
            Dim decryptor As ICryptoTransform = symmetricKey.CreateDecryptor(k1.GetBytes(16), initVectorBytes)
            Using plain As FileStream = File.Open(plainFilePath, FileMode.Create, FileAccess.Write, FileShare.None)
                Using encrypted As FileStream = File.Open(encryptedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using cs As New CryptoStream(plain, decryptor, CryptoStreamMode.Write)
                        encrypted.CopyTo(cs)
                    End Using
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine("[1007] Could not decrypt file: " + ex.ToString & vbCrLf)
        End Try
        Return False
    End Function

End Module
