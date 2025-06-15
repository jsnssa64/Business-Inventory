CREATE PROCEDURE [dbo].[SubscribeUserToWebhook]
	@Username VARCHAR(50),
	@WebhookUrl VARCHAR(50),
	@Secret VARCHAR(50),
	@SubscriptionType VARCHAR(50)
AS
	SET NOCOUNT ON;

    BEGIN TRY
        
        DECLARE @UserId INT;

        EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

        DECLARE @UserSubscriptionCount INT;

        SELECT @UserSubscriptionCount = COUNT(*) 
        FROM dbo.ServiceSubscription 
        WHERE UserId = @UserId
            AND TriggerAction = @SubscriptionType;

        IF( @UserSubscriptionCount > 0 )
            THROW 50000, 'User already subscribed to this webhook.', 1;

		INSERT INTO dbo.ServiceSubscription(TriggerAction, WebhookURI, UserId, SharedSecret)
		VALUES(@SubscriptionType, @WebhookUrl, @UserId, @Secret);

        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
