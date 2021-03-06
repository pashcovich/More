﻿namespace More.IO
{
    using System;
    using System.Threading.Tasks;

    internal sealed partial class StorageFileAdapter
    {
        public async Task<IFolder> GetParentAsync()
        {
            var parent = await file.GetParentAsync();
            return new StorageFolderAdapter( parent );
        }
    }
}
