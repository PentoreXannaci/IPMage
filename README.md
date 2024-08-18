# IPMage

**IPMage** is a tool for managing and updating your public IP address with DNS services. Currently, it supports Cloudflare, and it is designed to automatically update your DNS records if your public IP address changes.

## Features

- Fetches your current public IP address.
- Automatically updates DNS records with Cloudflare if the IP address changes.
- Configurable logging level.

## Requirements

- .NET 8.0 or later

## Configuration

The tool requires a `settings.json` file in the root of the application directory. The configuration file should have the following structure:

```json
{
  "logLevel": "information",
  "services": [
    {
      "serviceType": "cloudflare",
      "name": "e.g. domain.name (only used for logging)",
      "apiKey": "your-api-key",
      "zoneId": "your-zone-id",
      "recordId": "your-record-id"
    }
  ]
}
```

### Fields

- **logLevel**: Specifies the level of logging. Options include `debug`, `information`, `warning`, and `error`.
- **services**: An array of DNS service configurations.
  - **serviceType**: The type of DNS service. Currently, only `cloudflare` is supported.
  - **name**: A descriptive name for the domain (used only for logging).
  - **apiKey**: Your Cloudflare API key.
  - **zoneId**: The Cloudflare Zone ID for the domain.
  - **recordId**: The Cloudflare Record ID of the DNS record to update.

## Usage

1. Ensure that you have created a `settings.json` file in the root directory with the necessary configuration.
2. Execute the application. It will check your public IP address and update the Cloudflare DNS record if there is a change.

## Contributing

Feel free to submit issues or pull requests to improve the tool. Make sure to follow the standard GitHub workflow for contributions.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or feedback, please open an issue on the [GitHub repository](https://github.com/PentoreXannaci/IPMage).
