using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;

using Lionsguard.Configuration;
using Lionsguard.Data;
using Lionsguard.Providers;

namespace Lionsguard
{
	public static class Notification
	{
		private static bool _initialized = false;
		private static object _lock = new object();
		private static Exception _initException = null;

		#region Properties
		private static NotificationProvider _provider = null;
		private static NotificationProviderCollection _providers = null;
		private static string _adminEmail = "calbert@lionsguard.com";
		private static string _emailSubject = "Error on Lionsguard.com";
		private static string _smtpServer = "smtp.lionsguard.com";

		public static NotificationProvider Provider
		{
			get
			{
				Initialize();
				return _provider;
			}
		}

		public static NotificationProviderCollection Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}

		public static string AdminEmail
		{
			get
			{
				Initialize();
				return _adminEmail;
			}
		}

		public static string EmailSubject
		{
			get
			{
				Initialize();
				return _emailSubject;
			}
		}

		public static string SmtpServer
		{
			get
			{
				Initialize();
				return _smtpServer;
			}
		}
		#endregion

		#region Initialize
		private static void Initialize()
		{
			if (!_initialized)
			{
				lock (_lock)
				{
					if (!_initialized)
					{
						try
						{
							NotificationSection section = ConfigurationManager.GetSection("lionsguard/notification") as NotificationSection;
							if (section != null)
							{
								_providers = new NotificationProviderCollection();
								ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(NotificationProvider));
								_provider = _providers[section.DefaultProvider];
								if (_provider == null)
								{
									throw new ConfigurationErrorsException("Default NotificationProvider not found in application configuration file.", section.ElementInformation.Properties["defaultProvider"].Source, section.ElementInformation.Properties["defaultProvider"].LineNumber);
								}

								if (!String.IsNullOrEmpty(section.AdminEmail))
								{
									_adminEmail = section.AdminEmail;
								}
								if (!String.IsNullOrEmpty(section.EmailSubject))
								{
									_emailSubject = section.EmailSubject;
								}
								if (!String.IsNullOrEmpty(section.SmtpServer))
								{
									_smtpServer = section.SmtpServer;
								}
							}
						}
						catch (Exception ex)
						{
							_initException = ex;
						}
						_initialized = true;
					}
				}
			}
			if (_initException != null)
			{
				throw _initException;
			}
		}
		#endregion
		
		public static void SendEmail(string fromAddress, string toAddress, string subject, string message)
		{
			Notification.SendEmail(fromAddress, fromAddress, toAddress, toAddress, subject, message, MailPriority.Normal);
		}

		public static void SendEmail(string fromName, string fromAddress, string toName, string toAddress, string subject, string message)
		{
			Notification.SendEmail(new MailAddress(fromAddress, fromName), new MailAddress(toAddress, toName), subject, message, MailPriority.Normal);
		}

		public static void SendEmail(string fromName, string fromAddress, string toName, string toAddress, string subject, string message, MailPriority priority)
		{
			Notification.SendEmail(new MailAddress(fromAddress, fromName), new MailAddress(toAddress, toName), subject, message, priority);
		}

		public static void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, string message, MailPriority priority)
		{
			try
			{
				using (MailMessage msg = new MailMessage(fromAddress, toAddress))
				{
					msg.Subject = subject;
					msg.Body = message;
					msg.IsBodyHtml = true;
					msg.Priority = priority;

					SmtpClient smtp = new SmtpClient(Notification.SmtpServer, 25);
					smtp.Send(msg);
				}
			}
			catch (Exception ex)
			{
				Log.Write(ex.ToString());
			}
		}
	}
}
