ALTER TABLE [Users] ADD
    [SubscriptionPlan]          INT                 NOT NULL DEFAULT 0,
    [StripeCustomerId]          NVARCHAR(255)       NULL,
    [StripeSubscriptionId]      NVARCHAR(255)       NULL;