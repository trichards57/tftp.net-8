﻿// <copyright file="ReadOrWriteRequest.cs" company="Tony Richards">
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Tftp.Net;

internal abstract class ReadOrWriteRequest(ushort opCode, string filename, TftpTransferMode mode, IEnumerable<TransferOption> options)
{
    private readonly ushort opCode = opCode;

    public string Filename { get; private set; } = filename;

    public TftpTransferMode Mode { get; private set; } = mode;

    public IEnumerable<TransferOption> Options { get; private set; } = options;
}