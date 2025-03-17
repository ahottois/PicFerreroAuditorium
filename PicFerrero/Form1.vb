Imports System.Drawing.Imaging
Imports System.IO
Imports System.Xml

Public Class Form1
    Private originalImage As Image = Nothing
    Private lastUsedDirectory As String = ""
    Private settingsFilePath As String = ""

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize settings path
        Dim appDataFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ferrero\hottois\picture")

        ' Create directory if it doesn't exist
        If Not Directory.Exists(appDataFolder) Then
            Directory.CreateDirectory(appDataFolder)
        End If

        settingsFilePath = Path.Combine(appDataFolder, "settings.xml")

        ' Load settings if the file exists
        LoadSettings()

        ' Disable save button initially (until an image is loaded)
        Button2.Enabled = False
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Save settings before closing the application
        SaveSettings()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Load Picture button click handler
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp"
        openFileDialog.Title = "Select an Image"

        ' Use the last used directory if available
        If Not String.IsNullOrEmpty(lastUsedDirectory) AndAlso Directory.Exists(lastUsedDirectory) Then
            openFileDialog.InitialDirectory = lastUsedDirectory
        End If

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                ' Load the selected image
                originalImage = Image.FromFile(openFileDialog.FileName)
                PictureBox1.Image = originalImage

                ' Enable the Save button
                Button2.Enabled = True

                ' Remember the directory for next time
                lastUsedDirectory = Path.GetDirectoryName(openFileDialog.FileName)

                ' Set default values for width and height if not already set
                If String.IsNullOrEmpty(TextBox1.Text) Then
                    TextBox1.Text = originalImage.Height.ToString()
                End If

                If String.IsNullOrEmpty(TextBox2.Text) Then
                    TextBox2.Text = originalImage.Width.ToString()
                End If

                ' Update preview
                UpdatePreview()
            Catch ex As Exception
                MessageBox.Show("Error loading image: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Save Picture button click handler
        If originalImage Is Nothing Then
            MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp"
        saveFileDialog.Title = "Save the Modified Image"
        saveFileDialog.DefaultExt = "jpg"

        ' Use the last used directory if available
        If Not String.IsNullOrEmpty(lastUsedDirectory) AndAlso Directory.Exists(lastUsedDirectory) Then
            saveFileDialog.InitialDirectory = lastUsedDirectory
        End If

        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                ' Create and save the modified image
                Dim finalImage As Image = CreateFinalImage()

                ' Save with the appropriate format based on file extension
                Dim format As ImageFormat = ImageFormat.Jpeg
                Select Case Path.GetExtension(saveFileDialog.FileName).ToLower()
                    Case ".png"
                        format = ImageFormat.Png
                    Case ".bmp"
                        format = ImageFormat.Bmp
                    Case Else
                        format = ImageFormat.Jpeg
                End Select

                finalImage.Save(saveFileDialog.FileName, format)

                ' Remember the directory for next time
                lastUsedDirectory = Path.GetDirectoryName(saveFileDialog.FileName)

                ' Save settings
                SaveSettings()

                MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error saving image: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function CreateFinalImage() As Image
        ' Get parameters from controls
        Dim count As Integer = CInt(NumericUpDown1.Value)
        Dim singleHeight, singleWidth As Integer

        If Not Integer.TryParse(TextBox1.Text, singleHeight) OrElse singleHeight <= 0 Then
            singleHeight = originalImage.Height
        End If

        If Not Integer.TryParse(TextBox2.Text, singleWidth) OrElse singleWidth <= 0 Then
            singleWidth = originalImage.Width
        End If

        ' Adjust dimensions if keep ratio is checked
        If CheckBox1.Checked Then
            Dim originalRatio As Double = CDbl(originalImage.Width) / CDbl(originalImage.Height)

            ' If height was changed, adjust width
            If TextBox1.Modified AndAlso Not TextBox2.Modified Then
                singleWidth = CInt(singleHeight * originalRatio)
            Else ' Otherwise adjust height based on width
                singleHeight = CInt(singleWidth / originalRatio)
            End If
        End If

        ' Create a new bitmap with the calculated dimensions
        Dim totalWidth As Integer = singleWidth * count
        Dim resultImage As New Bitmap(totalWidth, singleHeight)

        Using g As Graphics = Graphics.FromImage(resultImage)
            g.Clear(Color.White) ' Set background color

            ' Draw the image the specified number of times
            For i As Integer = 0 To count - 1
                Dim destRect As New Rectangle(i * singleWidth, 0, singleWidth, singleHeight)

                ' Determine the source rectangle based on zoom option
                Dim srcRect As Rectangle
                If CheckBox2.Checked Then
                    ' Zoom: use the entire image stretched to fit the destination
                    srcRect = New Rectangle(0, 0, originalImage.Width, originalImage.Height)
                Else
                    ' No zoom: preserve aspect ratio and center
                    Dim srcWidth, srcHeight As Integer
                    Dim srcX, srcY As Integer

                    Dim originalRatio As Double = CDbl(originalImage.Width) / CDbl(originalImage.Height)
                    Dim destRatio As Double = CDbl(singleWidth) / CDbl(singleHeight)

                    If originalRatio > destRatio Then
                        ' Original is wider
                        srcHeight = originalImage.Height
                        srcWidth = CInt(srcHeight * destRatio)
                        srcX = (originalImage.Width - srcWidth) \ 2
                        srcY = 0
                    Else
                        ' Original is taller
                        srcWidth = originalImage.Width
                        srcHeight = CInt(srcWidth / destRatio)
                        srcX = 0
                        srcY = (originalImage.Height - srcHeight) \ 2
                    End If

                    srcRect = New Rectangle(srcX, srcY, srcWidth, srcHeight)
                End If

                ' Draw the image
                g.DrawImage(originalImage, destRect, srcRect, GraphicsUnit.Pixel)
            Next
        End Using

        Return resultImage
    End Function

    Private Sub UpdatePreview()
        ' Update the preview whenever settings change
        If originalImage IsNot Nothing Then
            Try
                Dim finalImage As Image = CreateFinalImage()

                ' Show preview in PictureBox2
                If PictureBox2.Image IsNot Nothing Then
                    PictureBox2.Image.Dispose()
                End If

                PictureBox2.Image = finalImage
            Catch ex As Exception
                ' Handle preview errors silently
            End Try
        End If
    End Sub

    ' Save application settings to XML file
    Private Sub SaveSettings()
        Try
            ' Create XML document
            Dim settings As New XmlDocument()
            Dim declaration As XmlDeclaration = settings.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
            settings.AppendChild(declaration)

            ' Create root element
            Dim root As XmlElement = settings.CreateElement("Settings")
            settings.AppendChild(root)

            ' Add last used directory
            Dim dirElement As XmlElement = settings.CreateElement("LastDirectory")
            dirElement.InnerText = lastUsedDirectory
            root.AppendChild(dirElement)

            ' Add number of copies
            Dim countElement As XmlElement = settings.CreateElement("CopyCount")
            countElement.InnerText = NumericUpDown1.Value.ToString()
            root.AppendChild(countElement)

            ' Add height
            Dim heightElement As XmlElement = settings.CreateElement("Height")
            heightElement.InnerText = TextBox1.Text
            root.AppendChild(heightElement)

            ' Add width
            Dim widthElement As XmlElement = settings.CreateElement("Width")
            widthElement.InnerText = TextBox2.Text
            root.AppendChild(widthElement)

            ' Add keep ratio setting
            Dim ratioElement As XmlElement = settings.CreateElement("KeepRatio")
            ratioElement.InnerText = CheckBox1.Checked.ToString()
            root.AppendChild(ratioElement)

            ' Add zoom setting
            Dim zoomElement As XmlElement = settings.CreateElement("ZoomPicture")
            zoomElement.InnerText = CheckBox2.Checked.ToString()
            root.AppendChild(zoomElement)

            ' Save the document
            settings.Save(settingsFilePath)
        Catch ex As Exception
            ' Handle silently - it's not critical if we can't save settings
            Debug.WriteLine("Error saving settings: " & ex.Message)
        End Try
    End Sub

    ' Load application settings from XML file
    Private Sub LoadSettings()
        Try
            If File.Exists(settingsFilePath) Then
                Dim settings As New XmlDocument()
                settings.Load(settingsFilePath)

                ' Get last used directory
                Dim dirNode As XmlNode = settings.SelectSingleNode("//LastDirectory")
                If dirNode IsNot Nothing Then
                    lastUsedDirectory = dirNode.InnerText
                End If

                ' Get number of copies
                Dim countNode As XmlNode = settings.SelectSingleNode("//CopyCount")
                If countNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(countNode.InnerText) Then
                    Dim count As Integer
                    If Integer.TryParse(countNode.InnerText, count) Then
                        NumericUpDown1.Value = Math.Max(NumericUpDown1.Minimum, Math.Min(NumericUpDown1.Maximum, count))
                    End If
                End If

                ' Get height
                Dim heightNode As XmlNode = settings.SelectSingleNode("//Height")
                If heightNode IsNot Nothing Then
                    TextBox1.Text = heightNode.InnerText
                End If

                ' Get width
                Dim widthNode As XmlNode = settings.SelectSingleNode("//Width")
                If widthNode IsNot Nothing Then
                    TextBox2.Text = widthNode.InnerText
                End If

                ' Get keep ratio setting
                Dim ratioNode As XmlNode = settings.SelectSingleNode("//KeepRatio")
                If ratioNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(ratioNode.InnerText) Then
                    Dim keepRatio As Boolean
                    If Boolean.TryParse(ratioNode.InnerText, keepRatio) Then
                        CheckBox1.Checked = keepRatio
                    End If
                End If

                ' Get zoom setting
                Dim zoomNode As XmlNode = settings.SelectSingleNode("//ZoomPicture")
                If zoomNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(zoomNode.InnerText) Then
                    Dim zoomPicture As Boolean
                    If Boolean.TryParse(zoomNode.InnerText, zoomPicture) Then
                        CheckBox2.Checked = zoomPicture
                    End If
                End If
            End If
        Catch ex As Exception
            ' Handle silently - it's not critical if we can't load settings
            Debug.WriteLine("Error loading settings: " & ex.Message)
        End Try
    End Sub

    ' Event handlers for control changes
    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        UpdatePreview()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If CheckBox1.Checked AndAlso originalImage IsNot Nothing AndAlso Integer.TryParse(TextBox1.Text, Nothing) Then
            Dim height As Integer = Integer.Parse(TextBox1.Text)
            Dim originalRatio As Double = CDbl(originalImage.Width) / CDbl(originalImage.Height)

            ' Update width to maintain ratio
            TextBox2.Text = CInt(height * originalRatio).ToString()
        End If
        UpdatePreview()
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If CheckBox1.Checked AndAlso originalImage IsNot Nothing AndAlso Integer.TryParse(TextBox2.Text, Nothing) Then
            Dim width As Integer = Integer.Parse(TextBox2.Text)
            Dim originalRatio As Double = CDbl(originalImage.Width) / CDbl(originalImage.Height)

            ' Update height to maintain ratio
            TextBox1.Text = CInt(width / originalRatio).ToString()
        End If
        UpdatePreview()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        ' When "keep ratio" is toggled, update dimensions
        If CheckBox1.Checked AndAlso originalImage IsNot Nothing Then
            Dim originalRatio As Double = CDbl(originalImage.Width) / CDbl(originalImage.Height)

            ' Update width based on current height
            If Integer.TryParse(TextBox1.Text, Nothing) Then
                Dim height As Integer = Integer.Parse(TextBox1.Text)
                TextBox2.Text = CInt(height * originalRatio).ToString()
            End If
        End If
        UpdatePreview()
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        UpdatePreview()
    End Sub
End Class