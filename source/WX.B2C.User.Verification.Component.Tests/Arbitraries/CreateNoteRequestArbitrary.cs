using System;
using FsCheck;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class CreateNoteRequestArbitrary : Arbitrary<CreateNoteRequest>
    {
        public static Arbitrary<CreateNoteRequest> Create() => new CreateNoteRequestArbitrary();

        public override Gen<CreateNoteRequest> Generator =>
            from subject in Arb.Generate<NoteSubject>()
            from text in StringGenerators.LettersOnly(1, 30)
            from subjectId in Arb.Generate<Guid>()
            select new CreateNoteRequest(text, subject, subjectId);
    }

    internal class InvalidNoteRequestArbitrary : Arbitrary<InvalidNoteRequest>
    {
        public static Arbitrary<InvalidNoteRequest> Create() => new InvalidNoteRequestArbitrary();

        public override Gen<InvalidNoteRequest> Generator =>
            from note in Arb.Generate<CreateNoteRequest>()
            from text in StringGenerators.LettersOnly(2001, 2500)
                                         .Or(Gen.Constant(string.Empty))
                                         .OrNull()
            select new InvalidNoteRequest(text, note.Subject, note.SubjectId);
    }
}
