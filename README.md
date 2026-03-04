# ThreadFileWriter

## Overview

ThreadFileWriter is a .NET 6 console application that demonstrates:

- Safe multithreaded programming
- Synchronized access to a shared file
- Asynchronous file I/O
- Proper exception handling
- Dependency Injection
- Docker containerization (Linux)

The application creates `/log/out.txt`, initializes it, then launches 10 concurrent tasks.  
Each task appends 10 lines to the file while guaranteeing sequential line numbering and safe file access.

---

## Functional Requirements Implemented

1. Create `/log/out.txt`
2. Write initial line:
   0, 0, HH:MM:SS.mmm
3. Launch 10 concurrent tasks
4. Each task appends 10 lines
5. Line numbers increment sequentially
6. Format:
   <line_number>, <thread_id>, <timestamp>
7. All tasks terminate gracefully
8. Wait for key press before exit

---

## Technology Stack

- .NET 6
- C#
- Async/Await
- System.Threading.Channels
- Dependency Injection
- Docker (Linux container)

---

## Thread Safety Strategy

- `Interlocked.Increment` ensures atomic line numbering.
- `Channel<T>` ensures single-writer pattern.
- Only one background consumer writes to the file.
- No race conditions.
- No file corruption.

---

## Exception Handling

The application safely handles:

### File Creation Failure
- Throws meaningful IOException
- Application exits with non-zero exit code

### Worker Task Exceptions
- Awaited via `Task.WhenAll`
- Exceptions propagate to Main
- Application exits cleanly

### Background Writer Failure
- Logged
- Rethrown
- Application exits gracefully

### Unhandled Exceptions
- Global exception handlers registered
- Errors logged to console

---

## Docker Instructions

### Build Docker Image

Run this from the project root (where Dockerfile exists):

```bash
docker build -t threadfilewriter .
```
### Run Docker Container

Run this from the project root (where Dockerfile exists):

```bash
docker run -i -v c:\junk:/log threadfilewriter
