CREATE PROCEDURE [dbo].[GetWebhooksByAction]
	@SubscriptionType VARCHAR(50)
AS
	SET NOCOUNT ON;

    BEGIN TRY
        
        SELECT DISTINCT s.WebhookURI, s.SharedSecret, s.TriggerAction
        FROM dbo.ServiceSubscription s
        JOIN dbo.[User] u ON s.UserId = u.Id
        WHERE u.[Disabled] = 0
            AND s.TriggerAction = @SubscriptionType;

        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @errMessage VARCHAR(1200) = 'Error: ' + CAST(ERROR_NUMBER() AS VARCHAR(100)) + ' at line ' + CAST(ERROR_LINE() AS VARCHAR(100)) + ' in ' + ISNULL(ERROR_PROCEDURE(), 'Ad-hoc') + ': ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0
