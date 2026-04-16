# uBrokenWindow

*The worst possible password reset utility, specially built for Umbraco.*

uBrokenWindow allows you to create a new backoffice user account on application startup, just by setting a few values in your appsettings.json file.

Yes, this is a horrible thing to install on your website, but sometimes you're put in a bad situation where you're given a database without a way to log into the backoffice. The Umbraco reset admin password process doesn't always work, this is much quicker (and dirtier).

I'd recommend installing this package only for only as long as necessary.

## Configuration

Add the following configuration to one of your appsettings.json files:

```json
{
  "uBrokenWindow": {
    "EnableBrokenWindow": false,
    "NewAdminEmail": "",
    "NewAdminPassword": ""
  }
}
```

On application startup, this package will check to ensure that the `EnableBrokenWindow` setting is set to `true`. A new user will be created only if an account with the provided email address does not already exist.

The username and email address will both bet set to the same value.

Check your application logs for confirmation and error messages.

### CAUTION

**DO NOT** commit the `NewAdminPassword` value to your repository.

## Cleanup

Once you're in, disable this package, and hell, even just uninstall it. Be sure to remove the configuration block, too.
