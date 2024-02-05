using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class FileToUploadArbitrary : Arbitrary<FileToUpload>
    {
        public static Arbitrary<FileToUpload> Create() => new FileToUploadArbitrary();

        public override Gen<FileToUpload> Generator =>
            from seed in Arb.Generate<Seed>()
            from files in FilesToUploadGenerators.Files(1, seed)
            select files.First();
    }

    internal class FileToUploadArrayArbitrary : Arbitrary<FileToUpload[]>
    {
        public static Arbitrary<FileToUpload[]> Create() => new FileToUploadArrayArbitrary();

        public override Gen<FileToUpload[]> Generator =>
            from seed in Arb.Generate<Seed>()
            from filesNumber in Gen.Choose(2, 10)
            from files in FilesToUploadGenerators.Files(filesNumber, seed)
            select files;
    }
}
