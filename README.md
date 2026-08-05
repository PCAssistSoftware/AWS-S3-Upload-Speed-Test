# AWS S3 Upload Speed Test

A small Windows Forms utility (VB.NET, .NET 10) for benchmarking upload throughput to an
Amazon S3 bucket. Generates random test data in memory — no real files are uploaded — and
reports the results.

## Features

- **Single upload test** — upload one file of a chosen size (1–500 MB) and see MB/s and Mbps.
- **Size series test** — uploads a range of sizes (1, 5, 10, 25, 50, 100, 250, 500 MB) back to
  back and reports one combined result across the whole run.
- **Small files test** — uploads a configurable number of files with random sizes within a
  min/max range (e.g. 100 files, 100 KB – 2 MB each), useful for measuring per-request overhead
  rather than raw bandwidth. Reports files/sec and files/hour alongside throughput.
- **Automatic region detection** — resolves the bucket's actual AWS region via the
  `x-amz-bucket-region` HTTP header (no IAM permission required), so uploads don't fail with a
  `PermanentRedirect` error if the wrong region is selected.
- **Configurable key prefix** — set the S3 key prefix uploads are written under, to match
  bucket policies that restrict access to a specific path.
- **Optional cleanup** — deletes test objects from the bucket after each run (on by default).

## Requirements

- Windows with the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download) (or the
  .NET 10 SDK to build from source).
- An AWS IAM access key/secret with permission to write to the target bucket.

## Building and running

```
dotnet build
dotnet run
```

Or open `AWSS3UploadSpeedTest.vbproj` in Visual Studio and run from there.

## Usage

1. Enter an AWS **Access Key ID** and **Secret Access Key**.
2. Enter the target **Bucket Name**. The region dropdown is a starting hint only — the app
   detects the bucket's real region automatically before uploading.
3. Optionally set a **Key Prefix** (default `speed-test/`) to match any bucket policy that
   scopes permissions to a specific prefix.
4. Choose a test type:
   - **Start Upload Test** — single file at the selected size.
   - **Run Size Series Test** — sweeps through multiple file sizes and reports one combined
     result.
   - **Run Small Files Test** — uploads many small files of random size (set count and
     min/max size in KB) and reports an aggregate result including files/sec and files/hour.
5. Results are appended to the log panel at the bottom, along with total size, total time,
   MB/s, Mbps, and (for the small files test) upload rate.

**MB/s** is raw throughput (megabytes/sec, matches the file size numbers directly). **Mbps**
is megabits/sec (1 MB/s = 8 Mbps) — the unit most ISPs advertise broadband speeds in, useful
for comparing against your connection's rated speed.

## Required IAM permissions

At minimum, the credentials used need `s3:PutObject` on the target bucket/prefix. If
"Delete test object(s) after upload" is left checked (the default), `s3:DeleteObject` is also
needed. `s3:GetBucketLocation` is optional but recommended (see below).

Note that `s3:GetBucketLocation` (and `s3:ListBucket`, if used) are **bucket-level** actions —
they only work when `Resource` is the bare bucket ARN (`arn:aws:s3:::YOUR-BUCKET-NAME`), not
the object-level wildcard (`arn:aws:s3:::YOUR-BUCKET-NAME/*`) used for `PutObject`/
`DeleteObject`. Mixing a bucket-level action into an object-scoped statement silently fails —
the action is listed but never actually matches, so it's still denied. Keep them in separate
statements:

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "BucketLevelAccess",
            "Effect": "Allow",
            "Action": [
                "s3:GetBucketLocation"
            ],
            "Resource": "arn:aws:s3:::YOUR-BUCKET-NAME"
        },
        {
            "Sid": "ObjectLevelAccess",
            "Effect": "Allow",
            "Action": [
                "s3:PutObject",
                "s3:DeleteObject"
            ],
            "Resource": "arn:aws:s3:::YOUR-BUCKET-NAME/*"
        }
    ]
}
```

`s3:GetBucketLocation` is not strictly required — region detection primarily uses a plain HTTP
header lookup that needs no IAM permission at all. It only falls back to the
`GetBucketLocation` API (and therefore this permission) if that header lookup fails for some
reason (network issue, non-standard endpoint, etc.). Granting it adds robustness but isn't
required for normal operation.
