using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;

namespace Lionsguard.Payments
{
	public static class Jambool
	{
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		public static string UserId
		{
			get { return ConfigurationManager.AppSettings["JamboolUserId"]; }
		}

		public static string OfferId
		{
			get { return ConfigurationManager.AppSettings["JamboolOfferId"]; }
		}

		public static string SecretKey
		{
			get { return ConfigurationManager.AppSettings["JamboolSecretKey"]; }
		}

		public static string CreateSignature(params KeyValuePair<string, string>[] parameters)
		{
			Array.Sort<KeyValuePair<string, string>>(parameters, new Comparison<KeyValuePair<string, string>>((p1, p2) =>
				{
					return p1.Key.CompareTo(p2.Key);
				}));
			var sb = new StringBuilder();
			foreach (var param in parameters)
			{
				sb.Append(param.Key).Append(param.Value);
			}
			sb.Append(SecretKey);

			return CreateHash(sb.ToString());
		}

		public static int GetTimestamp()
		{
			return (int)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
		}

		private static string CreateHash(string input)
		{
			var md5 = MD5.Create();

			var data = md5.ComputeHash(Encoding.Default.GetBytes(input));

			var sb = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sb.Append(data[i].ToString("x2"));
			}

			return sb.ToString();
		}
	}

	#region Java Class
//    /* =============================================================================
// *
// * copyright 2009 jambool
// *
// * ============================================================================
// * This program is free software; you can redistribute it and/or
// * modify it under the terms of the GNU General Public License,
// * version 2.
// *
// * This program is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// * =============================================================================
// */

///* =============================================================================
// *
// * sample client for the Social Gold Payments API
// *
// * =============================================================================
// */

//package com.jambool.socialgold.api;

//import java.net.Socket;
//import java.util.Hashtable;
//import java.util.List;
//import java.util.Enumeration;
//import java.util.Vector;
//import java.util.Collections;
//import java.io.UnsupportedEncodingException;
//import java.security.MessageDigest;
//import java.security.NoSuchAlgorithmException;
//import java.net.URLEncoder;
//import java.util.UUID;

//import org.apache.http.client.ResponseHandler;
//import org.apache.http.client.HttpClient;
//import org.apache.http.client.methods.HttpGet;
//import org.apache.http.impl.client.BasicResponseHandler;
//import org.apache.http.impl.client.DefaultHttpClient;
//import org.apache.http.HttpEntity;
//import org.apache.http.HttpResponse;
//import org.apache.http.StatusLine;
//import org.apache.http.util.EntityUtils;




///**
// * PaymentsClient
// * <p>
// * Sample code to provide convient access to the Social Gold Payments V1 API
// *
// * methods that make a call to the service:
// *  - getBalance
// *  - getTransactionStatus
// *  - getTransaction
// *  - creditUser
// *  - debitUser
// *  - refundUser
// *  - buyGoods
// *
// * methods that generate a URL for merchant to render in a page:
// *  - getBuyPaymentsURL
// *  - getBuyGoodsURL
// *
// * Notes:
// *  - input validation is weak, if at all
// *  - JSON, XML, and other parsers are not included at this time
// *  - formal excetions should be thrown 
// *
// */

//public class PaymentsClient {


//  private String offerID = null;
//  private String secretMerchantKey = null;
//  private String serverName = null;

//  /**
//   * -----------------------------------------------------------------------------
//   */

//  public PaymentsClient(String serverName, String offerID, String secretMerchantKey ) {

//    this.offerID = offerID;
//    this.secretMerchantKey = secretMerchantKey;
//    this.serverName = serverName;

//  }

//  // ==========================================================================
//  //
//  // buy_with_socialgold
//  //
//  // returns a URL to have in your IFRAME SRC= tag 
//  // 
//  // Reuired Params:
//  //
//  //  userID - string - the user_id of the player (e.g., "abc123") 
//  //  usdAmount - float - amount in USD$ of the purchase (e.g., 1.75 implies $1.75) 
//  //  currency_label - string - name of your currency (e.g., "Gold Coins") 
//  //  currency_xrate - integer - how much $1 is worth (e.g., 100 implies $1 = 100 Gold Coins) 
//  //  currency_amount - the amount of virtual currency in implied decimal (e.g., 10000 
//  //    "100" of the currency. max = 99999999999900). Cannot be used with "quantity" 
//  //  quantity - float - the amount of virtual currency without implied decimal (e.g. 100.99 
//  //    "100.99" of the currency. Up to 2 decimal places, max = 999999999999.00).  Cannot be used with "currency_amount" 
//  //
//  // Optional Params:
//  //  format ( iframe )
//  //  app_params - string - a variable that will be passed back to you on the postback 
//  //  platform - string - the platform of where this user is unique. If this is for a Facebook user, 
//  //    then pass in "facebook", etc. Use these values if appropiate, otherwise send in a string representing the platform 
//  //      facebook 
//  //      myspace 
//  //      orkut 
//  //      friendster 
//  //      hi5
//  //      tagged 
//  //      twitter 
//  //      bebo 
//  // You may also pass in any short (32 chars) alphanumeric string to represent your own platform. 
//  // For example "mywebsite", "mygame123" 


//  public String getBuyCurrencyURL(String userID, Float usdAmount, String currency_label, Integer currency_xrate, 
//            Integer currency_amount, Float quantity, String format, String app_params, String platform) throws Exception {
//    Hashtable baseParams = new Hashtable();
//    baseParams.put("action","buy_with_socialgold");
//    baseParams.put("offer_id",this.offerID);
//    baseParams.put("user_id",userID);
//    baseParams.put("format",format); // TODO: validate format is valid
//    Hashtable requiredParams = new Hashtable();
//    requiredParams.put("currency_label",currency_label);
//    if (usdAmount != null) requiredParams.put("amount",usdAmount); 
//    if (currency_amount != null) requiredParams.put("currency_amount",currency_amount);
//    if (currency_xrate != null) requiredParams.put("currency_xrate",currency_xrate);
//    if (quantity != null) requiredParams.put("quantity",quantity); 
//    Hashtable optionalParams = new Hashtable();
//    if (app_params != null) optionalParams.put("app_params",app_params);
//    if (platform != null) optionalParams.put("platform",platform);
//    String cmdURL = _getURL(baseParams,requiredParams,optionalParams,true);
//    return cmdURL;
//  }


//  // ==========================================================================
//  //
//  // buy_goods_with_socialgold
//  //
//  // returns a URL to have in your IFRAME SRC= tag 
//  // 
//  // Reuired Params:
//  //
//  //  userID - string - the user_id of the player (e.g., "abc123") 
//  //  usdAmount - float - amount in USD$ of the purchase (e.g., 1.75 implies $1.75) 
//  //  title - string - description of item being sold
//  //
//  // Optional Params:
//  //  format ( iframe )
//  //  app_params - string - a variable that will be passed back to you on the postback 
//  //  external_ref_id - string -  a unique string with your representation of the transaction
//  //  quantity - int - default=1
//  //  platform - string - the platform of where this user is unique. If this is for a Facebook user, 
//  //    then pass in "facebook", etc. Use these values if appropiate, otherwise send in a string representing the platform 
//  //      facebook 
//  //      myspace 
//  //      orkut 
//  //      friendster 
//  //      hi5
//  //      tagged 
//  //      twitter 
//  //      bebo 
//  //   You may also pass in any short (32 chars) alphanumeric string to represent your own platform. 
//  //   For example "mywebsite", "mygame123" 
//  //  platform_sub_id - int - unique enum, representing the above platforms - see website for details
//  //  statement_descriptor - 8 character string - to be shown on invice, credit card statement, etc
//  //  sku - string - unique string representing item in your system

//  public String  getBuyGoodsURL(String userID, Float usdAmount, String title,
//            String format, String external_ref_id, Integer quantity, String app_params,
//            String platform, String platform_sub_id, String statement_descriptor, String sku)  throws Exception {

//    Hashtable baseParams = new Hashtable();
//    baseParams.put("action","buy_goods_with_socialgold");
//    baseParams.put("offer_id",this.offerID);
//    baseParams.put("user_id",userID);
//    baseParams.put("format",format); // TODO: validate format is valid
//    Hashtable requiredParams = new Hashtable();
//    requiredParams.put("amount",usdAmount); 
//    requiredParams.put("title",title);
//    Hashtable optionalParams = new Hashtable();
//    if (external_ref_id != null) optionalParams.put("external_ref_id",external_ref_id);
//    if (app_params != null) optionalParams.put("app_params",app_params);
//    if (platform != null) optionalParams.put("platform",platform);
//    if (quantity != null) optionalParams.put("quantity",quantity);
//    if (platform_sub_id != null) optionalParams.put("platform_sub_id",platform_sub_id);
//    if (statement_descriptor != null) optionalParams.put("statement_descriptor",statement_descriptor);
//    if (sku != null) optionalParams.put("sku",sku);
//    String cmdURL = _getURL(baseParams,requiredParams,optionalParams,true);
//    return cmdURL;
//  }



//  /**
//   * =============================================================================
//   * private methods below this 
//   * =============================================================================
//   */

//  private String _getURL(Hashtable baseParams, Hashtable requiredParams, Hashtable optionalParams) {
//    return  _getURL(baseParams, requiredParams, optionalParams,false);
//  }
//  private String _getURL(Hashtable baseParams, Hashtable requiredParams, Hashtable optionalParams, Boolean requiresSSL) {

//    String userID = baseParams.get("user_id").toString();
//    String action = baseParams.get("action").toString();
//    String format = baseParams.get("format").toString();
//    long timestamp = System.currentTimeMillis() / 1000;

//    Hashtable signatureParams = (Hashtable) baseParams.clone();
//    signatureParams.remove("format"); // format is NOT in signature
//    signatureParams.remove("action"); // action is NOT in signature
//    signatureParams.put("ts",timestamp);
//    _mergeParams(requiredParams, signatureParams);
//    _mergeParams(optionalParams, signatureParams);

//    String signature = _calculateSignature(signatureParams);
//    StringBuffer uri = new StringBuffer("/payments/v1/"+this.offerID+"/"+action+"/?sig="+signature+"&ts="+timestamp+"&format="+format+"&user_id="+userID);
//    uri.append(_paramHashToURI(requiredParams));
//    uri.append(_paramHashToURI(optionalParams));

//    String proto = ( requiresSSL ) ? "https://" : "http://";
//    String url = proto+this.serverName+uri.toString();
//    return url;
//  }

//  /**
//   * -----------------------------------------------------------------------------
//   */

//  private void _mergeParams(Hashtable srcParams, Hashtable destParams) {

//    if (srcParams == null) {
//      destParams = new Hashtable();
//    }
//    if (srcParams != null) {
//      Object key = null;
//      for ( Enumeration keys = srcParams.keys(); keys.hasMoreElements(); ) {
//        key = keys.nextElement();
//        destParams.put(key, srcParams.get(key));
//      }
//    }
//  }    

//  /**
//   * -----------------------------------------------------------------------------
//   */

//  private String _paramHashToURI(Hashtable params) {

//    StringBuffer result = new StringBuffer();
//    if (params != null) {
//      Object key = null;
//      Object value = null;
//      for ( Enumeration keys = params.keys(); keys.hasMoreElements(); ) {
//        key = keys.nextElement();
//        value = params.get(key);
//        if ( value != null && (value = value.toString().trim()) != "") {
//          String v = "";
//          try {
//            v = URLEncoder.encode(value.toString(),"UTF-8");
//            result.append("&"+key.toString()+"="+v);
//          } catch (Exception e) {
//          }
//        }
//      }
//    }
//    return result.toString();
//  }

//  /**
//   * -----------------------------------------------------------------------------
//   */

//  private String _calculateSignature(Hashtable signatureParams) {
//    String signature = null;
//    StringBuffer sigSrc = new StringBuffer();
//    if (signatureParams != null) {
//      Object key = null;
//      Object value = null;

//      // System.out.println("sigParams = |"+signatureParams+"|"); 

//      Vector keyVector = new Vector(signatureParams.keySet());
//      Collections.sort(keyVector);

//      for ( Enumeration keys = keyVector.elements(); keys.hasMoreElements(); ) {
//        key = keys.nextElement();
//        value = signatureParams.get(key);
//        if ( value != null ) {
//          sigSrc.append(key.toString()+value.toString());
//        }
//      }
//    }

//    sigSrc.append(this.secretMerchantKey);
//    String sigSrcString=sigSrc.toString();
//    // System.out.println("sigSrc = |"+sigSrcString+"|"); // WARNING - shows secretMerchantKey - for debug only!!!

//    try {
//      byte[] md5hash = MessageDigest.getInstance("MD5").digest(sigSrcString.getBytes("UTF-8"));

//      StringBuilder hexString = new StringBuilder(md5hash.length * 2);
//      for (byte b : md5hash) {
//        if ((b & 0xff) < 0x10) hexString.append("0");
//        hexString.append(Long.toString(b & 0xff, 16));
//      }

//      signature = hexString.toString();
//      // System.out.println("signature = |"+signature+"|"); 

//    } catch (Exception e) {
//      System.err.println("MD5 related excpetion" + e);
//      signature = "";
//    }

//    return signature;
//  }

//  /**
//   * -----------------------------------------------------------------------------
//   */

//  private Hashtable _getResponse(String cmdURL) throws Exception {
//      return _getResponse(cmdURL,false);
//  }

//  private Hashtable _getResponse(String cmdURL, Boolean requiresSSL) throws Exception {
//    Hashtable result = new Hashtable();
//    StatusLine status = null;
//    String body = null;

//    System.out.println("Getting : " + cmdURL);
//    try {

//      HttpClient httpclient = new DefaultHttpClient();
//      HttpGet httpget = new HttpGet(cmdURL); 
//      try { 
//        // DefaultHttpParams.getDefaultParams().setParameter("http.protocol.cookie-policy", CookiePolicy.BROWSER_COMPATIBILITY);
//        HttpResponse response = httpclient.execute(httpget);
//        HttpEntity entity = response.getEntity();

//        status = response.getStatusLine();
//        if (entity != null) {
//            body = EntityUtils.toString(entity);
//        }

//      } finally {
//        httpclient.getConnectionManager().shutdown();        
//      }
  
//      // System.out.println("==============");
//      // System.out.println("Response: status=" + status);
//      // System.out.println("Response: body=" + body);
//      // System.out.println("==============");
//      result.put("body",body);
//      int statusCode = status.getStatusCode();
//      result.put("status.code",statusCode);
//      if (statusCode != 200 ) {
//        result.put("error.code",statusCode); // TODO - get real code from body
//        // TODO throw exception ?
//      }
//    } catch (Exception e) {
//      System.out.println("got an exception "+e);
//      throw e;
//    }
//    return result;
//  }


//  /**
//   * =============================================================================
//   */
//  public static void main(String[] args) throws Exception {

//    System.out.println("BEGIN");

//    String offerID = "socialgolddemoofferid456";
//    String secretMerchantKey = "abcdefghijklmnopqrstuvwx";
//    String apiServerName = "api.sandbox.jambool.com";

//    Hashtable result = null;
//    String userID = null;
//    String format="iframe";
//    String url=null;


//    try {
//      PaymentsClient paymentsClient = new PaymentsClient( apiServerName, 
//          offerID, 
//          secretMerchantKey);

//    userID = "demo_user_id";
//    Float usdAmount=new Float(10.0);
//    String currency_label="myJavaBucks";
//    Integer currency_xrate=null;
//    Integer currency_amount=null;
//    Float quantity=null;
//    String app_params="myGame";
//    String platform="facebook";

//      // ---  getBuyCurrencyURL  -----------------------------------------------

//      System.out.println("");
//      System.out.println("getBuyPaymentsURL URL :");
//      System.out.println("");
//      System.out.println("With currency_xrate: ");
//      currency_xrate=new Integer(10); 
//      currency_amount=null;
//      quantity=null;
//      url = paymentsClient.getBuyCurrencyURL(userID, usdAmount, currency_label, currency_xrate, currency_amount, quantity, format, app_params, platform );
//      System.out.println("URL = "+url);

//      System.out.println("With currency_amount: ");
//      currency_xrate=null;
//      currency_amount=new Integer(100); 
//      quantity=null;
//      url = paymentsClient.getBuyCurrencyURL(userID, usdAmount, currency_label, currency_xrate, currency_amount, quantity, format, app_params, platform );
//      System.out.println("URL = "+url);

//      System.out.println("With quantity: ");
//      currency_xrate=null;
//      currency_amount=null;
//      quantity=new Float(3.0);
//      url = paymentsClient.getBuyCurrencyURL(userID, usdAmount, currency_label, currency_xrate, currency_amount, quantity, format, app_params, platform );
//      System.out.println("URL = "+url);


//      System.out.println("");
//      System.out.println("getBuyGoodsURL URL :");
//      System.out.println("");

//      String title = "java client";
//      String sku = null;
//      // sku = "java123";
//      String platform_sub_id = null;
//      String statement_descriptor = null;
//      String external_ref_id = UUID.randomUUID().toString();
//      Integer iQuantity=null;
//      // iQuantity=new Integer(2);

//      url = paymentsClient.getBuyGoodsURL(userID, usdAmount, title,
//            format, external_ref_id, iQuantity, app_params,
//            platform, platform_sub_id, statement_descriptor, sku );

//      System.out.println("URL = "+url);


//    } finally {
//      System.out.println("");
//      System.out.println("DONE");
//    }
//  }

//}


///*
//* =============================================================================
//* EOF
//* ============================================================================= 
//*/



	#endregion
}
