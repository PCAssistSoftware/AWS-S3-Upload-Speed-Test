Imports Amazon
Imports Amazon.Runtime
Imports Amazon.S3
Imports Amazon.S3.Model
Imports Amazon.S3.Transfer
Imports System.Diagnostics
Imports System.Linq
Imports System.Net.Http
Imports System.Threading

Public Class Form1

    Private ReadOnly _regions As String() = {
        "us-east-1", "us-east-2", "us-west-1", "us-west-2",
        "af-south-1", "ap-east-1", "ap-south-1",
        "ap-northeast-1", "ap-northeast-2", "ap-northeast-3",
        "ap-southeast-1", "ap-southeast-2", "ap-southeast-3",
        "ca-central-1",
        "eu-central-1", "eu-west-1", "eu-west-2", "eu-west-3", "eu-north-1", "eu-south-1",
        "me-south-1", "sa-east-1"
    }

    Private ReadOnly _fileSizesMB As Integer() = {1, 5, 10, 25, 50, 100, 250, 500}

    Private _cts As CancellationTokenSource
    Private _stopwatch As Stopwatch

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim iconPath = IO.Path.Combine(AppContext.BaseDirectory, "icon.ico")
        If IO.File.Exists(iconPath) Then
            Icon = New Drawing.Icon(iconPath)
        End If

        cboRegion.Items.AddRange(_regions)
        cboRegion.SelectedItem = "eu-north-1"

        For Each size As Integer In _fileSizesMB
            cboFileSize.Items.Add($"{size} MB")
        Next
        cboFileSize.SelectedIndex = 2 ' 10 MB

        txtKeyPrefix.Text = "speed-test/"
    End Sub

    Private Sub chkShowSecret_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowSecret.CheckedChanged
        txtSecretKey.PasswordChar = If(chkShowSecret.Checked, ControlChars.NullChar, "*"c)
    End Sub

    Private Sub btnClearLog_Click(sender As Object, e As EventArgs) Handles btnClearLog.Click
        txtLog.Clear()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        _cts?.Cancel()
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        If Not ValidateInputs() Then Return

        RunUploadTest()
    End Sub

    Private Sub btnRunSeries_Click(sender As Object, e As EventArgs) Handles btnRunSeries.Click
        If Not ValidateInputs() Then Return

        RunSeriesTest()
    End Sub

    Private Sub btnRunSmallFiles_Click(sender As Object, e As EventArgs) Handles btnRunSmallFiles.Click
        If Not ValidateInputs() Then Return

        RunSmallFilesTest()
    End Sub

    Private Function ValidateInputs() As Boolean
        If String.IsNullOrWhiteSpace(txtAccessKey.Text) OrElse String.IsNullOrWhiteSpace(txtSecretKey.Text) Then
            MessageBox.Show("Please enter both an Access Key ID and Secret Access Key.", "Missing Credentials", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If cboRegion.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a region.", "Missing Region", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If String.IsNullOrWhiteSpace(txtBucket.Text) Then
            MessageBox.Show("Please enter a bucket name.", "Missing Bucket", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Async Sub RunUploadTest()
        SetRunningState(True)
        _cts = New CancellationTokenSource()

        Try
            Using client = Await PrepareClientAsync()
                Dim bucketName = txtBucket.Text.Trim()
                Dim sizeMB = _fileSizesMB(cboFileSize.SelectedIndex)
                Await UploadOnceAsync(client, bucketName, sizeMB, _cts.Token)
            End Using

        Catch ex As OperationCanceledException
            lblStatus.Text = "Upload cancelled."
            AppendLog($"{DateTime.Now:HH:mm:ss}  Cancelled after {_stopwatch.Elapsed.TotalSeconds:F2}s")

        Catch ex As AmazonS3Exception
            lblStatus.Text = "Upload failed (see log)."
            AppendLog($"{DateTime.Now:HH:mm:ss}  S3 ERROR: {ex.ErrorCode} - {ex.Message}")
            MessageBox.Show(ex.Message, "S3 Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Catch ex As Exception
            lblStatus.Text = "Upload failed (see log)."
            AppendLog($"{DateTime.Now:HH:mm:ss}  ERROR: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            _cts?.Dispose()
            _cts = Nothing
            SetRunningState(False)
        End Try
    End Sub

    Private Async Sub RunSeriesTest()
        SetRunningState(True)
        _cts = New CancellationTokenSource()

        Try
            Using client = Await PrepareClientAsync()
                Dim bucketName = txtBucket.Text.Trim()

                Dim totalBytes As Long = 0
                Dim totalSeconds As Double = 0

                For i = 0 To _fileSizesMB.Length - 1
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim sizeMB = _fileSizesMB(i)
                    lblStatus.Text = $"Series test {i + 1}/{_fileSizesMB.Length}: {sizeMB} MB"
                    Dim result = Await UploadOnceAsync(client, bucketName, sizeMB, _cts.Token, logResult:=False)
                    totalBytes += result.Bytes
                    totalSeconds += result.Seconds
                Next

                Dim overallMBps = (totalBytes / (1024.0 * 1024.0)) / totalSeconds
                Dim overallMbitPerSec = overallMBps * 8.0
                Dim filesPerSec = _fileSizesMB.Length / totalSeconds
                Dim filesPerHour = filesPerSec * 3600.0

                lblStatus.Text = $"Series done. {FormatBytes(totalBytes)} in {totalSeconds:F2}s  →  {overallMBps:F2} MB/s ({overallMbitPerSec:F1} Mbps) overall"

                AppendResult($"SERIES RESULT: {_fileSizesMB.Length} files ({String.Join(", ", _fileSizesMB.Select(Function(s) $"{s}MB"))})",
                    $"Total size:  {FormatBytes(totalBytes)}",
                    $"Total time:  {totalSeconds:F2}s",
                    $"Speed:       {overallMBps:F2} MB/s  ({overallMbitPerSec:F1} Mbps)",
                    $"Rate:        {filesPerSec:F2} files/s  ({filesPerHour:N0} files/hr)")
            End Using

        Catch ex As OperationCanceledException
            lblStatus.Text = "Series cancelled."
            AppendLog($"{DateTime.Now:HH:mm:ss}  Series cancelled.")

        Catch ex As AmazonS3Exception
            lblStatus.Text = "Upload failed (see log)."
            AppendLog($"{DateTime.Now:HH:mm:ss}  S3 ERROR: {ex.ErrorCode} - {ex.Message}")
            MessageBox.Show(ex.Message, "S3 Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Catch ex As Exception
            lblStatus.Text = "Upload failed (see log)."
            AppendLog($"{DateTime.Now:HH:mm:ss}  ERROR: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            _cts?.Dispose()
            _cts = Nothing
            SetRunningState(False)
        End Try
    End Sub

    Private Async Sub RunSmallFilesTest()
        SetRunningState(True)
        _cts = New CancellationTokenSource()

        Try
            Using client = Await PrepareClientAsync()
                Dim bucketName = txtBucket.Text.Trim()
                Dim fileCount = CInt(numFileCount.Value)
                Dim minSizeKB = CInt(numMinSizeKB.Value)
                Dim maxSizeKB = CInt(numMaxSizeKB.Value)
                If minSizeKB > maxSizeKB Then
                    Dim swap = minSizeKB
                    minSizeKB = maxSizeKB
                    maxSizeKB = swap
                End If
                Dim maxSizeBytes As Long = CLng(maxSizeKB) * 1024L
                Dim seed = CInt(numSeed.Value)

                Dim prefix = txtKeyPrefix.Text.Trim()
                If prefix.Length > 0 AndAlso Not prefix.EndsWith("/") Then
                    prefix &= "/"
                End If
                Dim batchId = $"smallfiles-{DateTime.Now:yyyyMMdd-HHmmss}"

                ' One buffer sized to the largest possible file; each upload uses a
                ' random-length slice of it so every file gets a different random size.
                ' The buffer content itself doesn't need to be reproducible - only the
                ' sequence of sizes does, so runs with the same settings are comparable
                ' across machines. That uses its own seeded generator, below.
                Dim buffer(maxSizeBytes - 1) As Byte
                Await Task.Run(Sub() Random.Shared.NextBytes(buffer))

                Dim sizeRandom As New Random(seed)
                Dim uploadedKeys As New List(Of String)
                Dim totalBytes As Long = 0
                Dim overallStopwatch = Stopwatch.StartNew()

                For i = 1 To fileCount
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim sizeKB = sizeRandom.Next(minSizeKB, maxSizeKB + 1)
                    Dim sizeBytes = CLng(sizeKB) * 1024L
                    Dim key = $"{prefix}{batchId}/{Environment.MachineName}-{i:0000}-{sizeKB}KB.bin"

                    lblStatus.Text = $"Small files: {i}/{fileCount} ({sizeKB} KB)"
                    progressBar1.Value = CInt(i * 100L / fileCount)

                    Using ms As New IO.MemoryStream(buffer, 0, CInt(sizeBytes))
                        Dim request = New PutObjectRequest With {
                            .BucketName = bucketName,
                            .Key = key,
                            .InputStream = ms,
                            .ContentType = "application/octet-stream",
                            .AutoCloseStream = False
                        }
                        Await client.PutObjectAsync(request, _cts.Token)
                    End Using

                    uploadedKeys.Add(key)
                    totalBytes += sizeBytes
                Next

                overallStopwatch.Stop()

                Dim seconds = overallStopwatch.Elapsed.TotalSeconds
                Dim mbPerSec = (totalBytes / (1024.0 * 1024.0)) / seconds
                Dim mbitPerSec = mbPerSec * 8.0
                Dim filesPerSec = fileCount / seconds
                Dim filesPerHour = filesPerSec * 3600.0

                progressBar1.Value = 100
                lblStatus.Text = $"Done. {fileCount} files uploaded in {seconds:F2}s  →  {mbPerSec:F2} MB/s, {filesPerSec:F1} files/s"

                AppendResult($"SMALL FILES RESULT: {fileCount} files ({minSizeKB}-{maxSizeKB} KB each, seed {seed})",
                    $"Total size:  {FormatBytes(totalBytes)}",
                    $"Total time:  {seconds:F2}s",
                    $"Speed:       {mbPerSec:F2} MB/s  ({mbitPerSec:F1} Mbps)",
                    $"Rate:        {filesPerSec:F1} files/s  ({filesPerHour:N0} files/hr)")

                If chkDeleteAfter.Checked Then
                    lblStatus.Text = "Cleaning up test files..."
                    Await DeleteObjectsBatchAsync(client, bucketName, uploadedKeys)
                End If
            End Using

        Catch ex As OperationCanceledException
            lblStatus.Text = "Small files test cancelled."
            AppendLog($"{DateTime.Now:HH:mm:ss}  Small files test cancelled.")

        Catch ex As AmazonS3Exception
            lblStatus.Text = "Upload failed (see log)."
            AppendLog($"{DateTime.Now:HH:mm:ss}  S3 ERROR: {ex.ErrorCode} - {ex.Message}")
            MessageBox.Show(ex.Message, "S3 Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Catch ex As Exception
            lblStatus.Text = "Upload failed (see log)."
            AppendLog($"{DateTime.Now:HH:mm:ss}  ERROR: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            _cts?.Dispose()
            _cts = Nothing
            SetRunningState(False)
        End Try
    End Sub

    ''' S3 DeleteObjects accepts at most 1000 keys per request, so batch in chunks.
    Private Async Function DeleteObjectsBatchAsync(client As AmazonS3Client, bucketName As String, keys As List(Of String)) As Task
        Dim offset = 0
        While offset < keys.Count
            Dim chunk = keys.Skip(offset).Take(1000).ToList()
            Dim request = New DeleteObjectsRequest With {
                .BucketName = bucketName,
                .Objects = chunk.Select(Function(k) New KeyVersion With {.Key = k}).ToList()
            }
            Await client.DeleteObjectsAsync(request)
            offset += 1000
        End While
    End Function

    ''' Resolves the bucket's real region (updating the dropdown to match) and
    ''' builds an S3 client for it. Shared by both the single test and the series test.
    Private Async Function PrepareClientAsync() As Task(Of AmazonS3Client)
        Dim credentials = New BasicAWSCredentials(txtAccessKey.Text.Trim(), txtSecretKey.Text.Trim())
        Dim bucketName = txtBucket.Text.Trim()

        lblStatus.Text = "Detecting bucket region..."
        Dim selectedRegion = RegionEndpoint.GetBySystemName(cboRegion.SelectedItem.ToString())
        Dim actualRegion = Await ResolveBucketRegionAsync(credentials, bucketName, selectedRegion)

        If _regions.Contains(actualRegion.SystemName) Then
            cboRegion.SelectedItem = actualRegion.SystemName
        End If

        Dim config = New AmazonS3Config With {.RegionEndpoint = actualRegion}
        Return New AmazonS3Client(credentials, config)
    End Function

    ''' Generates sizeMB of random data, uploads it, optionally logs the result, and
    ''' optionally deletes it again. Used for both single and series tests.
    ''' Returns the bytes uploaded and elapsed seconds so callers can aggregate.
    Private Async Function UploadOnceAsync(client As AmazonS3Client, bucketName As String, sizeMB As Integer, token As CancellationToken, Optional logResult As Boolean = True) As Task(Of (Bytes As Long, Seconds As Double))
        Dim sizeBytes As Long = CLng(sizeMB) * 1024L * 1024L

        Dim prefix = txtKeyPrefix.Text.Trim()
        If prefix.Length > 0 AndAlso Not prefix.EndsWith("/") Then
            prefix &= "/"
        End If
        Dim key = $"{prefix}{Environment.MachineName}-{DateTime.Now:yyyyMMdd-HHmmss}-{sizeMB}MB.bin"

        lblStatus.Text = $"Generating {sizeMB} MB test data..."
        progressBar1.Value = 0

        Dim data(sizeBytes - 1) As Byte
        Await Task.Run(Sub() Random.Shared.NextBytes(data))

        _stopwatch = Stopwatch.StartNew()

        Using ms As New IO.MemoryStream(data)
            Dim uploadRequest = New TransferUtilityUploadRequest With {
                .BucketName = bucketName,
                .Key = key,
                .InputStream = ms,
                .ContentType = "application/octet-stream"
            }
            AddHandler uploadRequest.UploadProgressEvent, AddressOf OnUploadProgress

            Using transferUtility As New TransferUtility(client)
                Await transferUtility.UploadAsync(uploadRequest, token)
            End Using
        End Using

        _stopwatch.Stop()

        Dim seconds = _stopwatch.Elapsed.TotalSeconds
        Dim mbPerSec = (sizeBytes / (1024.0 * 1024.0)) / seconds
        Dim mbitPerSec = mbPerSec * 8.0

        progressBar1.Value = 100
        lblStatus.Text = $"Done. {sizeMB} MB uploaded in {seconds:F2}s  →  {mbPerSec:F2} MB/s ({mbitPerSec:F1} Mbps)"

        If logResult Then
            AppendResult($"UPLOAD RESULT: {sizeMB} MB",
                $"Time:   {seconds:F2}s",
                $"Speed:  {mbPerSec:F2} MB/s  ({mbitPerSec:F1} Mbps)",
                $"Key:    {key}")
        End If

        If chkDeleteAfter.Checked Then
            Await client.DeleteObjectAsync(bucketName, key)
        End If

        Return (sizeBytes, seconds)
    End Function

    ''' Buckets can live in a different region than the one picked in the UI.
    ''' S3 reports a bucket's real region via the x-amz-bucket-region header on
    ''' a plain HEAD request - no IAM permission needed for that, so try it
    ''' first. Fall back to the GetBucketLocation API call (needs s3:GetBucketLocation
    ''' on the bucket ARN), then to the UI selection, otherwise uploads fail with a
    ''' PermanentRedirect / "must be addressed using the specified endpoint" error.
    Private Async Function ResolveBucketRegionAsync(credentials As BasicAWSCredentials, bucketName As String, fallbackRegion As RegionEndpoint) As Task(Of RegionEndpoint)
        Dim headerRegion = Await TryGetBucketRegionFromHeaderAsync(bucketName)
        If Not String.IsNullOrEmpty(headerRegion) Then
            Try
                Return RegionEndpoint.GetBySystemName(headerRegion)
            Catch
            End Try
        End If

        Using probeClient As New AmazonS3Client(credentials, RegionEndpoint.USEast1)
            Try
                Dim response = Await probeClient.GetBucketLocationAsync(bucketName)
                Dim locationValue = response.Location.Value
                If String.IsNullOrEmpty(locationValue) Then
                    Return RegionEndpoint.USEast1
                End If
                Return RegionEndpoint.GetBySystemName(locationValue)
            Catch
                Return fallbackRegion
            End Try
        End Using
    End Function

    Private Async Function TryGetBucketRegionFromHeaderAsync(bucketName As String) As Task(Of String)
        Try
            Using httpClient As New HttpClient()
                httpClient.Timeout = TimeSpan.FromSeconds(5)
                Using request As New HttpRequestMessage(HttpMethod.Head, $"https://{bucketName}.s3.amazonaws.com/")
                    Using response = Await httpClient.SendAsync(request)
                        Dim values As IEnumerable(Of String) = Nothing
                        If response.Headers.TryGetValues("x-amz-bucket-region", values) Then
                            Return values.FirstOrDefault()
                        End If
                    End Using
                End Using
            End Using
        Catch
        End Try
        Return Nothing
    End Function

    Private Sub OnUploadProgress(sender As Object, e As UploadProgressArgs)
        If InvokeRequired Then
            BeginInvoke(Sub() OnUploadProgress(sender, e))
            Return
        End If

        progressBar1.Value = Math.Min(e.PercentDone, 100)

        Dim elapsed = _stopwatch.Elapsed.TotalSeconds
        If elapsed > 0 Then
            Dim avgMBps = (e.TransferredBytes / (1024.0 * 1024.0)) / elapsed
            lblStatus.Text = $"Uploading: {e.PercentDone}%  ({FormatBytes(e.TransferredBytes)} / {FormatBytes(e.TotalBytes)})  Avg: {avgMBps:F2} MB/s"
        End If
    End Sub

    Private Sub SetRunningState(running As Boolean)
        btnStart.Enabled = Not running
        btnRunSeries.Enabled = Not running
        btnRunSmallFiles.Enabled = Not running
        btnCancel.Enabled = running
        grpCredentials.Enabled = Not running
        grpTest.Enabled = Not running
        grpSmallFiles.Enabled = Not running
        If running Then
            progressBar1.Value = 0
        End If
    End Sub

    Private Sub AppendLog(line As String)
        txtLog.AppendText(line & Environment.NewLine)
    End Sub

    ''' Writes a timestamped heading followed by one indented line per result field,
    ''' with a blank line after for separation between test runs.
    Private Sub AppendResult(heading As String, ParamArray fields() As String)
        Dim sb As New Text.StringBuilder()
        sb.AppendLine($"{DateTime.Now:HH:mm:ss}  {heading}")
        For Each field In fields
            sb.AppendLine($"    {field}")
        Next
        sb.AppendLine()
        txtLog.AppendText(sb.ToString())
    End Sub

    Private Shared Function FormatBytes(bytes As Long) As String
        If bytes >= 1024L * 1024L Then
            Return $"{bytes / (1024.0 * 1024.0):F2} MB"
        ElseIf bytes >= 1024L Then
            Return $"{bytes / 1024.0:F2} KB"
        Else
            Return $"{bytes} B"
        End If
    End Function

End Class
