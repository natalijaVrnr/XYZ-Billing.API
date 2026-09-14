using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ_Billing.API.Domain.Models;

namespace XYZ.Billing.API.Persistence.Configurations;

internal class PaymentsConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        throw new NotImplementedException();
    }
}
