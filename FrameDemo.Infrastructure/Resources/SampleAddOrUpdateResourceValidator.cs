using FluentValidation;

namespace FrameDemo.Infrastructure.Resources
{
    public class SampleAddOrUpdateResourceValidator<T> : AbstractValidator<T> where T : SampleAddOrUpdateResource
    {
        public SampleAddOrUpdateResourceValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(50)
                .WithName("标题")
                .WithMessage("maxlength|{PropertyName}的最大长度是{MaxLength}");

            RuleFor(x => x.Body)
                .NotNull()
                .WithName("正文")
                .WithMessage("required|{PropertyName}是必填的")
                .MinimumLength(10)
                .WithMessage("minlength|{PropertyName}的最小长度是{MinLength}");
        }
    }
}