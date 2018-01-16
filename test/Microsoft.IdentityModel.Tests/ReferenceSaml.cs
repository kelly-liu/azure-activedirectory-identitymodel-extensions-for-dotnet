//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Xml;

namespace Microsoft.IdentityModel.Tests
{
    public class ReferenceSaml
    {
        public static SamlAssertion SamlAssertion
        {
            get
            {
                var reference = new Reference
                {
                    DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256",
                    DigestValue = "JaDhvSguu/XZ8jZmh7KmhbOr4deZB4/iL1adETm9oPc=",
                    TokenStream = Default.TokenStream,
                    Uri = "#091a00cc-4361-4303-9f1a-d4be45b2b84c"
                };

                reference.Transforms.Add(new EnvelopedSignatureTransform());
                reference.CanonicalizingTransfrom = new ExclusiveCanonicalizationTransform();

                var signature = new Signature
                {
                    KeyInfo = new KeyInfo(KeyingMaterial.AADSigningCert),
                    SignatureValue = "NRV7REVbDRflg616G6gYg0fAGTEw8BhtyPzqaU+kPQI35S1vpgt12VlQ57PkY7Rs0Jucx9npno+bQVMKN2DNhhnzs9qoNY2V3TcdJCcwaMexinHoFXHA0+J6+vR3RWTXhX+iAnfudtKThqbh/mECRLrjyTdy6L+qNkP7sALCWrSVwJVRmzkTOUF8zG4AKY9dQziec94Zv4S7G3cFgj/i7ok2DfBi7AEMCu1lh3dsQAMDeCvt7binhIH2D2ad3iCfYyifDGJ2ncn9hIyxrEiBdS8hZzWijcLs6+HQhVaz9yhZL9u/ZxSRaisXClMdqrLFjUghJ82sVfgQdp7SF165+Q==",
                    SignedInfo = new SignedInfo(reference)
                };

                var assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditions, null, new Collection<SamlStatement> { SamlAttributeStatement })
                {
                    Signature = signature
                };

                return assertion;
            }
        }

        public static SamlConditions SamlConditions
        {
            get
            {
                var audiences = new Uri(Default.Audience);
                var conditions = new Collection<SamlCondition> { new SamlAudienceRestrictionCondition(audiences) };
                return new SamlConditions(Default.NotBefore, Default.NotOnOrAfter, conditions);
            }
        }

        public static SamlSubject SamlSubject
        {
            get => new SamlSubject(string.Empty, string.Empty, string.Empty, new string[] { Default.SamlConfirmationMethod }, string.Empty);
        }

        public static SamlAttributeStatement SamlAttributeStatement
        {
            get => GetAttributeStatement(SamlSubject, ClaimSets.DefaultClaims);
        }

        public static SamlSecurityToken SamlSecurityToken
        {
            get => new SamlSecurityToken(SamlAssertion);
        }

        public static SamlAttributeStatement GetAttributeStatement(SamlSubject subject, IEnumerable<Claim> claims)
        {
            string defaultNamespace = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
            Collection<SamlAttribute> attributes = new Collection<SamlAttribute>();
            foreach (var claim in claims)
            {
                string type = claim.Type;
                string name = type;
                if (type.Contains("/"))
                {
                    int lastSlashIndex = type.LastIndexOf('/');
                    name = type.Substring(lastSlashIndex + 1);
                }

                type = defaultNamespace;

                string value = claim.Value;
                SamlAttribute attribute = new SamlAttribute(type, name, claim.Value);
                attributes.Add(attribute);
            }

            return new SamlAttributeStatement(subject, attributes);
        }

        #region Saml

        #region SamlAction
        public static SamlActionTestSet SamlActionValueNull
        {
            get
            {
                return new SamlActionTestSet
                {
                    Xml = XmlGenerator.SamlActionXml(SamlConstants.Namespace, Default.SamlAction.Namespace.ToString(), null)
                };
            }
        }

        public static SamlActionTestSet SamlActionValueEmptyString
        {
            get
            {
                return new SamlActionTestSet
                {
                    Xml = XmlGenerator.SamlActionXml(SamlConstants.Namespace, Default.SamlAction.Namespace.ToString(), String.Empty)
                };
            }
        }

        public static SamlActionTestSet SamlActionNamespaceNull
        {
            get
            {
                return new SamlActionTestSet
                {
                    Action = Default.SamlAction,
                    Xml = XmlGenerator.SamlActionXml(SamlConstants.Namespace, null, Default.SamlAction.Value)
                };
            }
        }

        public static SamlActionTestSet SamlActionNamespaceEmptyString
        {
            get
            {
                return new SamlActionTestSet
                {
                    Action = Default.SamlAction,
                    Xml = XmlGenerator.SamlActionXml(SamlConstants.Namespace, string.Empty, Default.SamlAction.Value)
                };
            }
        }

        public static SamlActionTestSet SamlActionNamespaceNotAbsoluteUri
        {
            get
            {
                return new SamlActionTestSet
                {
                    Xml = XmlGenerator.SamlActionXml(SamlConstants.Namespace, "namespace", Default.SamlAction.Value)
                };
            }
        }

        public static SamlActionTestSet SamlActionValid
        {
            get
            {
                return new SamlActionTestSet
                {
                    Action = Default.SamlAction,
                    Xml = XmlGenerator.SamlActionXml(SamlConstants.Namespace, Default.SamlAction.Namespace.ToString(), Default.SamlAction.Value)
                };
            }
        }

        public static SamlActionTestSet SamlActionEmpty
        {
            get
            {
                return new SamlActionTestSet
                {
                    Action = Default.SamlAction,
                    Xml = "<Action Namespace=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns=\"urn:oasis:names:tc:SAML:1.0:assertion\"/>"
                };
            }
        }

        #endregion

        #region SamlAdvice
        public static SamlAdviceTestSet AdviceNoAssertionIDRefAndAssertion
        {
            get
            {
                return new SamlAdviceTestSet
                {
                    Advice = new SamlAdvice(),
                    Xml = XmlGenerator.SamlAdviceXml(null, null)
                };
            }
        }

        public static SamlAdviceTestSet AdviceWithAssertionIDRef
        {
            get
            {
                return new SamlAdviceTestSet
                {
                    Advice = new SamlAdvice(new string[] { Default.SamlAssertionID }),
                    Xml = XmlGenerator.SamlAdviceXml(XmlGenerator.SamlAssertionIDRefXml(Default.SamlAssertionID), null)
                };
            }
        }

        public static SamlAdviceTestSet SamlAdviceWithAssertions
        {
            get
            {
                return new SamlAdviceTestSet
                {
                    Advice = new SamlAdvice(new List<SamlAssertion> { SamlAssertionNoSignature.Assertion }),
                    Xml = XmlGenerator.SamlAdviceXml(null, SamlAssertionNoSignature.Xml)
                };
            }
        }

        public static SamlAdviceTestSet SamlAdviceWithWrongElement
        {
            get
            {
                return new SamlAdviceTestSet
                {
                    Xml = XmlGenerator.SamlAdviceXml(SamlActionValid.Xml, null)
                };
            }
        }

        public static SamlAdviceTestSet SamlAdviceWithAssertionIDRefAndAssertions
        {
            get
            {
                return new SamlAdviceTestSet
                {
                    Advice = new SamlAdvice(new string[] { Default.SamlAssertionID }, new List<SamlAssertion> { SamlAssertionNoSignature.Assertion }),
                    Xml = XmlGenerator.SamlAdviceXml(XmlGenerator.SamlAssertionIDRefXml(Default.SamlAssertionID), SamlAssertionNoSignature.Xml)
                };
            }
        }
        #endregion

        #region SamlAssertion
        public static SamlAssertionTestSet SamlAssertionMissMajorVersion
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(null, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionEmpty
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MinorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, null, null, null, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionWrongMajorVersion
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(Convert.ToString(2), SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMissMinorVersion
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, null, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionWrongMinorVersion
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, Convert.ToString(2), Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMissAssertionID
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, null, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionWrongAssertionID
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, "12345", Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMissIssuer
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, null, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMissIssuerInstant
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, null, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionNoCondition
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), null, AdviceWithAssertionIDRef.Advice, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement }),
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, null, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionNoAdvice
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditionsSingleCondition.Conditions, null, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement }),
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, null, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMissStatement
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, null, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionWrongElementInStatementPlace
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlActionValid.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionNoSignature
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditionsSingleCondition.Conditions, AdviceWithAssertionIDRef.Advice, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement }),
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionWithSignature
        {
            get
            {
                var signatureXml = @"<ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/><ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256""/><ds:Reference Id=""_b95759d0-73ae-4072-a140-567ade10a7ad"" URI=""#_b95759d0-73ae-4072-a140-567ade10a7ad""><ds:Transforms><ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/><ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/></ds:Transforms><ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""/><ds:DigestValue>NLCLU+vIJShFuQF8kGFSShWFmYXhj1XDA5vBR+BSHdI=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>QyDvaRhV1EzJE0z0rsJY5nayt5jjLZlxDH4daqPPqnfRG288D0aMx4Q2hd7iAjf0YJPWOjDkjIwkogX+GyPo4EICm3QO4G7N0gNqAy7vG8WtnXCKwSFe/lNXi3TYf3uSLXRUWaNrpCM2LXDx9hti1I7ybNeDS0OnuOAQmiF0sU5cuC0ewOKKOqpBVGPF6QM4wsf9/PFhgxAyWPtr+je3mmXC7BsICZFmHplD3EaS9p6vxZ3Ld8FV5S4VhxB0+soM5b7RhYRgHRcz/nJyycRyqZgG2TqnG3jJMN1rIbLJ1asE26AXWGU/4G8UD2iZKy8SYHKH1WGzhQa1xN1fXxch9g==</ds:SignatureValue><ds:KeyInfo><ds:X509Data><ds:X509Certificate>MIIDJTCCAg2gAwIBAgIQGzlg2gNmfKRKBa6dqqZXxzANBgkqhkiG9w0BAQQFADAiMSAwHgYDVQQDExdLZXlTdG9yZVRlc3RDZXJ0aWZpY2F0ZTAeFw0xMTExMDkxODE5MDZaFw0zOTEyMzEyMzU5NTlaMCIxIDAeBgNVBAMTF0tleVN0b3JlVGVzdENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAns1cm8RU1hKZILPI6pB5Zoxn9mW2tSS0atV+o9FCn9NyeOktEOj1kEXOeIz0KfnqxgPMF1GpshuZBAhgjkyy2kNGE6Zx50CCJgq6XUatvVVJpMp8/FV18ynPf+/TRlF8V2HO3IVJ0XqRJ9fGA2f5xpOweWsdLYitdHbaDCl6IBNSXo52iNuqWAcB1k7jBlsnlXpuvslhLIzj60dnghAVA4ltS3NlFyw1Tz3pGlZQDt7x83IBHe7DA9bV3aJs1trkm1NzI1HoRS4vOqU3n4fn+DlfAE2vYKNkSi/PjuAX+1YQCq6e5uN/hOeSEqji8SsWC2nk/bMTKPwD67rn3jNC9wIDAQABo1cwVTBTBgNVHQEETDBKgBA3gSuALjvEuAVmF/x8knXvoSQwIjEgMB4GA1UEAxMXS2V5U3RvcmVUZXN0Q2VydGlmaWNhdGWCEBs5YNoDZnykSgWunaqmV8cwDQYJKoZIhvcNAQEEBQADggEBAFZvDA7PBh/vvFZb/QCBelTyD2Yqij16v3tk30A3Akli6UIILdbbOcA5BiPktT1kJxcsgSXNHUODlfG2Fy9HTqwunr8G7FYniOUXPVrRL+HwhKOzRFDMUS3+On+ZDzum7rbpm3SYlnJDyNb8wynPw/bXQw72jGjt63uh6OnkYE8fJ8iPfVWOenZkP/IXPIXK/bBwLMDJ1y77ZauPYbp7oiQ/991pn0c7F4ugT9LYmbAdJKhiainOaoBTvIHN8/lMZ8gHUuxvOJhPrbgo3NTqvT1/3kfD0AISP4R3pH0QL/0m7cO34nK4rFFLZs1sFUguYUJhfkyq1N8MiyyAqRmrvBQ=</ds:X509Certificate></ds:X509Data></ds:KeyInfo></ds:Signature>";
                var dsigSerializer = DSigSerializer.Default;
                var assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditionsSingleCondition.Conditions, AdviceWithAssertionIDRef.Advice, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement });
                assertion.SigningCredentials = Default.AsymmetricSigningCredentials;
                assertion.Signature = dsigSerializer.ReadSignature(XmlUtilities.CreateDictionaryReader(signatureXml));
                assertion.Signature.SignedInfo.References[0].TokenStream = Default.TokenStream;
                return new SamlAssertionTestSet
                {
                    Assertion = assertion,
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, SamlAttributeStatementSingleAttribute.Xml, signatureXml)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMultiStatements_SameSubject
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditionsSingleCondition.Conditions, AdviceWithAssertionIDRef.Advice, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement, SamlAttributeStatementSingleAttribute.AttributeStatement }),
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, string.Concat(SamlAttributeStatementSingleAttribute.Xml, SamlAttributeStatementSingleAttribute.Xml), null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMultiStatements_DifferentSubject
        {
            get
            {
                SamlSubject subject = new SamlSubject(Default.NameIdentifierFormat, Default.NameQualifier, Default.Subject);
                SamlAttributeStatement statement = new SamlAttributeStatement(subject, SamlAttributeSingleValue.Attribute);

                return new SamlAssertionTestSet
                {
                    Assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditionsSingleCondition.Conditions, AdviceWithAssertionIDRef.Advice, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement, statement }),
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml,
                        string.Concat(SamlAttributeStatementSingleAttribute.Xml, XmlGenerator.SamlAttributeStatementXml(XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, Default.Subject), null), SamlAttributeSingleValue.Xml)), null)
                };
            }
        }

        public static SamlAssertionTestSet SamlAssertionMultiStatements_DifferentStatementType
        {
            get
            {
                return new SamlAssertionTestSet
                {
                    Assertion = new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), SamlConditionsSingleCondition.Conditions, AdviceWithAssertionIDRef.Advice, new List<SamlStatement> { SamlAttributeStatementSingleAttribute.AttributeStatement, SamlAuthenticationStatementValid.AuthenticationStatement }),
                    Xml = XmlGenerator.SamlAssertionXml(SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue, Default.SamlAssertionID, Default.Issuer, Default.IssueInstantString, SamlConditionsSingleCondition.Xml, AdviceWithAssertionIDRef.Xml, string.Concat(SamlAttributeStatementSingleAttribute.Xml, SamlAuthenticationStatementValid.Xml), null)
                };
            }
        }
        #endregion

        #region SamlAttribute
        public static SamlAttributeTestSet SamlAttributeNameNull
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Xml = XmlGenerator.SamlAttributeXml(null, Default.AttributeNamespace, new List<string> { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country) })
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeNameEmptyString
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Xml = XmlGenerator.SamlAttributeXml(string.Empty, Default.AttributeNamespace, new List<string> { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country) })
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeNamespaceNull
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Xml = XmlGenerator.SamlAttributeXml(Default.AttributeName, null, new List<string> { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country) })
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeNamespaceEmptyString
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Xml = XmlGenerator.SamlAttributeXml(Default.AttributeName, string.Empty, new List<string> { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country) })
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeValueNull
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Attribute =  Default.SamlAttributeNoValue,
                    Xml = XmlGenerator.SamlAttributeXml(Default.AttributeName, Default.AttributeNamespace, null)
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeValueEmptyString
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Attribute = new SamlAttribute(Default.AttributeNamespace, Default.AttributeName, new string[] { string.Empty }),
                    Xml = XmlGenerator.SamlAttributeXml(Default.AttributeName, Default.AttributeNamespace, new List<string> { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, string.Empty) })
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeSingleValue
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Attribute = Default.SamlAttributeSingleValue,
                    Xml = XmlGenerator.SamlAttributeXml(Default.AttributeName, Default.AttributeNamespace, new List<string> { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country) })
                };
            }
        }

        public static SamlAttributeTestSet SamlAttributeMultiValue
        {
            get
            {
                return new SamlAttributeTestSet
                {
                    Attribute = Default.SamlAttributeMultiValue,
                    Xml = XmlGenerator.SamlAttributeXml(Default.AttributeName, Default.AttributeNamespace, new List<string>
                            { XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country),
                              XmlGenerator.SamlAttributeValueXml(SamlConstants.Namespace, Default.Country)  })
                };
            }
        }
        #endregion

        #region SamlAttributeStatement
        public static SamlAttributeStatementTestSet SamlAttributeStatementMissSubject
        {
            get
            {
                return new SamlAttributeStatementTestSet
                {
                    Xml = XmlGenerator.SamlAttributeStatementXml(null, SamlAttributeSingleValue.Xml)
                };
            }
        }

        public static SamlAttributeStatementTestSet SamlAttributeStatementMissAttribute
        {
            get
            {
                return new SamlAttributeStatementTestSet
                {
                    Xml = XmlGenerator.SamlAttributeStatementXml(SamlSubjectWithNameIdentifierAndConfirmation.Xml, null)
                };
            }
        }

        public static SamlAttributeStatementTestSet SamlAttributeStatementSingleAttribute
        {
            get
            {
                return new SamlAttributeStatementTestSet
                {
                    AttributeStatement = new SamlAttributeStatement(SamlSubjectWithNameIdentifierAndConfirmation.Subject, SamlAttributeSingleValue.Attribute),
                    Xml = XmlGenerator.SamlAttributeStatementXml(SamlSubjectWithNameIdentifierAndConfirmation.Xml, SamlAttributeSingleValue.Xml)
                };
            }
        }

        public static SamlAttributeStatementTestSet SamlAttributeStatementMultiAttributes
        {
            get
            {
                return new SamlAttributeStatementTestSet
                {
                    AttributeStatement = new SamlAttributeStatement(SamlSubjectWithNameIdentifierAndConfirmation.Subject,
                                new List<SamlAttribute> { SamlAttributeSingleValue.Attribute, SamlAttributeSingleValue.Attribute }),
                    Xml = XmlGenerator.SamlAttributeStatementXml(SamlSubjectWithNameIdentifierAndConfirmation.Xml, string.Concat(SamlAttributeSingleValue.Xml, SamlAttributeSingleValue.Xml))
                };
            }
        }
        #endregion

        #region SamlAuthenticationStatement
        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMissSubject
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, null, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                        XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMissMethod
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(null, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                        XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMissInstant
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, null, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                        XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementNoSubjectLocality
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    AuthenticationStatement = new SamlAuthenticationStatement(SamlSubjectNoNameQualifier.Subject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), null, null,
                                        new List<SamlAuthorityBinding> { new SamlAuthorityBinding(new System.Xml.XmlQualifiedName(Default.AuthorityKind), Default.Location, Default.Binding) }),
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, null,
                                        XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementNoIPAddress
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    AuthenticationStatement = new SamlAuthenticationStatement(SamlSubjectNoNameQualifier.Subject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), Default.DNSAddress, string.Empty,
                                        new List<SamlAuthorityBinding> { new SamlAuthorityBinding(new System.Xml.XmlQualifiedName(Default.AuthorityKind), Default.Location, Default.Binding) }),
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(null, Default.DNSAddress),
                                XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementNoDNSAddress
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    AuthenticationStatement = new SamlAuthenticationStatement(SamlSubjectNoNameQualifier.Subject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), string.Empty, Default.IPAddress,
                                        new List<SamlAuthorityBinding> { new SamlAuthorityBinding(new System.Xml.XmlQualifiedName(Default.AuthorityKind), Default.Location, Default.Binding) }),
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, null),
                                XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementNoAuthorityBinding
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    AuthenticationStatement = new SamlAuthenticationStatement(SamlSubjectNoNameQualifier.Subject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), Default.DNSAddress, Default.IPAddress, null),
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress), null)
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMissAuthorityKind
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                XmlGenerator.SamlAuthorityBindingXml(null, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMissLocation
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, null, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMissBinding
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, null))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementEmpty
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    Xml = @"<AuthenticationStatement AuthenticationMethod=""urn:oasis:names:tc:SAML:1.0:am:password"" AuthenticationInstant =""2017-03-18T18:33:37.080Z"" xmlns=""urn:oasis:names:tc:SAML:1.0:assertion""/>"
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementValid
        {
            get
            {
                return new SamlAuthenticationStatementTestSet
                {
                    AuthenticationStatement = new SamlAuthenticationStatement(SamlSubjectNoNameQualifier.Subject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), Default.DNSAddress, Default.IPAddress,
                                        new List<SamlAuthorityBinding> { new SamlAuthorityBinding(new System.Xml.XmlQualifiedName(Default.AuthorityKind), Default.Location, Default.Binding) }),
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding))
                };
            }
        }

        public static SamlAuthenticationStatementTestSet SamlAuthenticationStatementMultiBinding
        {
            get
            {
                string authorityBinding = XmlGenerator.SamlAuthorityBindingXml(Default.AuthorityKind, Default.Location, Default.Binding);
                SamlAuthorityBinding binding = new SamlAuthorityBinding(new System.Xml.XmlQualifiedName(Default.AuthorityKind), Default.Location, Default.Binding);
                return new SamlAuthenticationStatementTestSet
                {
                    AuthenticationStatement = new SamlAuthenticationStatement(SamlSubjectNoNameQualifier.Subject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), Default.DNSAddress, Default.IPAddress,
                                        new List<SamlAuthorityBinding> { binding, binding }),
                    Xml = XmlGenerator.SamlAuthenticationStatementXml(Default.AuthenticationMethod, Default.AuthenticationInstant, SamlSubjectNoNameQualifier.Xml, XmlGenerator.SamlSubjectLocalityXml(Default.IPAddress, Default.DNSAddress),
                                string.Concat(authorityBinding, authorityBinding))
                };
            }
        }
        #endregion

        #region SamlAudienceRestrictionCondition
        public static SamlAudienceRestrictionConditionTestSet SamlAudienceRestrictionConditionNoAudience
        {
            get
            {
                return new SamlAudienceRestrictionConditionTestSet
                {
                    Xml = XmlGenerator.SamlAudienceRestrictionConditionXml(new string[] { })
                };
            }
        }

        public static SamlAudienceRestrictionConditionTestSet SamlAudienceRestrictionConditionEmptyAudience
        {
            get
            {
                return new SamlAudienceRestrictionConditionTestSet
                {
                    Xml = XmlGenerator.SamlAudienceRestrictionConditionXml(new string[] { XmlGenerator.SamlAudienceXml(string.Empty) })
                };
            }
        }

        public static SamlAudienceRestrictionConditionTestSet SamlAudienceRestrictionConditionInvaidElement
        {
            get
            {
                return new SamlAudienceRestrictionConditionTestSet
                {
                    Xml = XmlGenerator.SamlAudienceRestrictionConditionXml(new string[] { XmlGenerator.SamlActionXml(null, null, null) })
                };
            }
        }

        public static SamlAudienceRestrictionConditionTestSet SamlAudienceRestrictionConditionSingleAudience
        {
            get
            {
                return new SamlAudienceRestrictionConditionTestSet
                {
                    AudienceRestrictionCondition = Default.SamlAudienceRestrictionConditionSingleAudience,
                    Xml = XmlGenerator.SamlAudienceRestrictionConditionXml(new string[] { XmlGenerator.SamlAudienceXml(Default.Audience) })
                };
            }
        }

        public static SamlAudienceRestrictionConditionTestSet SamlAudienceRestrictionConditionMultiAudience
        {
            get
            {
                var audiences = new List<string>();
                foreach (var audience in Default.Audiences)
                {
                    audiences.Add(XmlGenerator.SamlAudienceXml(audience));
                }

                return new SamlAudienceRestrictionConditionTestSet
                {
                    AudienceRestrictionCondition = Default.SamlAudienceRestrictionConditionMultiAudience,
                    Xml = XmlGenerator.SamlAudienceRestrictionConditionXml(audiences)
                };
            }
        }
        #endregion

        #region SamlAuthorizationDecisionStatement
        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionMissResource
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(null, Default.SamlAccessDecision.ToString(), SamlSubjectWithNameIdentifierAndConfirmation.Xml, SamlActionValid.Xml, SamlEvidenceWithAssertionIDRef.Xml)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionMissAccessDecision
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(Default.SamlResource, null, SamlSubjectWithNameIdentifierAndConfirmation.Xml, SamlActionValid.Xml, SamlEvidenceWithAssertionIDRef.Xml)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionMissSubject
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(Default.SamlResource, Default.SamlAccessDecision.ToString(), null, SamlActionValid.Xml, SamlEvidenceWithAssertionIDRef.Xml)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionMissAction
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(Default.SamlResource, Default.SamlAccessDecision.ToString(), SamlSubjectWithNameIdentifierAndConfirmation.Xml, null, SamlEvidenceWithAssertionIDRef.Xml)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionNoEvidence
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    AuthorizationDecision = new SamlAuthorizationDecisionStatement(SamlSubjectWithNameIdentifierAndConfirmation.Subject, Default.SamlResource, Default.SamlAccessDecision, new List<SamlAction> { SamlActionValid.Action }),
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(Default.SamlResource, Default.SamlAccessDecision.ToString(), SamlSubjectWithNameIdentifierAndConfirmation.Xml, SamlActionValid.Xml, null)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionSingleAction
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    AuthorizationDecision = new SamlAuthorizationDecisionStatement(SamlSubjectWithNameIdentifierAndConfirmation.Subject, Default.SamlResource, Default.SamlAccessDecision, new List<SamlAction> { SamlActionValid.Action }, SamlEvidenceWithAssertionIDRef.Evidence),
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(Default.SamlResource, Default.SamlAccessDecision.ToString(), SamlSubjectWithNameIdentifierAndConfirmation.Xml, SamlActionValid.Xml, SamlEvidenceWithAssertionIDRef.Xml)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionMultiActions
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    AuthorizationDecision = new SamlAuthorizationDecisionStatement(SamlSubjectWithNameIdentifierAndConfirmation.Subject, Default.SamlResource, Default.SamlAccessDecision, new List<SamlAction> { SamlActionValid.Action, SamlActionValid.Action }, SamlEvidenceWithAssertionIDRef.Evidence),
                    Xml = XmlGenerator.SamlAuthorizationDecisionStatementXml(Default.SamlResource, Default.SamlAccessDecision.ToString(), SamlSubjectWithNameIdentifierAndConfirmation.Xml, string.Concat(SamlActionValid.Xml, SamlActionValid.Xml), SamlEvidenceWithAssertionIDRef.Xml)
                };
            }
        }

        public static SamlAuthorizationDecisionStatementTestSet SamlAuthorizationDecisionEmpty
        {
            get
            {
                return new SamlAuthorizationDecisionStatementTestSet
                {
                    Xml = @"<AuthorizationDecisionStatement Resource=""http://www.w3.org/"" Decision=""Permit"" xmlns=""urn:oasis:names:tc:SAML:1.0:assertion""/>"
                };
            }
        }
        #endregion

        #region SamlConditions
        public static SamlConditionsTestSet SamlConditionsNoNbf
        {
            get
            {
                return new SamlConditionsTestSet
                {
                    Conditions = new SamlConditions(DateTimeUtil.GetMinValue(DateTimeKind.Utc), Default.NotOnOrAfter, new List<SamlCondition> { Default.SamlAudienceRestrictionConditionSingleAudience }),
                    Xml = XmlGenerator.SamlConditionsXml(null, Default.NotOnOrAfterString, new List<string> { SamlAudienceRestrictionConditionSingleAudience.Xml })
                };
            }
        }

        public static SamlConditionsTestSet SamlConditionsNoNotOnOrAfter
        {
            get
            {
                return new SamlConditionsTestSet
                {
                    Conditions = new SamlConditions(Default.NotBefore, DateTimeUtil.GetMaxValue(DateTimeKind.Utc), new List<SamlCondition> { Default.SamlAudienceRestrictionConditionSingleAudience }),
                    Xml = XmlGenerator.SamlConditionsXml(Default.NotBeforeString, null, new List<string> { SamlAudienceRestrictionConditionSingleAudience.Xml })
                };
            }
        }

        public static SamlConditionsTestSet SamlConditionsNoCondition
        {
            get
            {
                return new SamlConditionsTestSet
                {
                    Conditions = new SamlConditions(DateTimeUtil.GetMinValue(DateTimeKind.Utc), DateTimeUtil.GetMaxValue(DateTimeKind.Utc)),
                    Xml = XmlGenerator.SamlConditionsXml(null, null, null)
                };
            }
        }

        public static SamlConditionsTestSet SamlConditionsSingleCondition
        {
            get
            {
                return new SamlConditionsTestSet
                {
                    Conditions = Default.SamlConditionsSingleCondition,
                    Xml = XmlGenerator.SamlConditionsXml(Default.NotBeforeString, Default.NotOnOrAfterString, new List<string> { SamlAudienceRestrictionConditionSingleAudience.Xml })
                };
            }
        }

        public static SamlConditionsTestSet SamlConditionsMultiCondition
        {
            get
            {
                return new SamlConditionsTestSet
                {
                    Conditions = new SamlConditions(Default.NotBefore, Default.NotOnOrAfter, new List<SamlCondition>
                        { Default.SamlAudienceRestrictionConditionSingleAudience,
                          Default.SamlAudienceRestrictionConditionMultiAudience }),
                    Xml = XmlGenerator.SamlConditionsXml(Default.NotBeforeString, Default.NotOnOrAfterString,
                            new List<string> { SamlAudienceRestrictionConditionSingleAudience.Xml, SamlAudienceRestrictionConditionMultiAudience.Xml })
                };
            }
        }

        public static SamlConditionsTestSet SamlConditionsEmpty
        {
            get
            {
                return new SamlConditionsTestSet
                {
                    Conditions = new SamlConditions(Default.NotBefore, Default.NotOnOrAfter, null),
                    Xml = @"<Conditions NotBefore=""2017-03-17T18:33:37.080Z"" NotOnOrAfter=""2017-03-18T18:33:37.080Z"" xmlns=""urn:oasis:names:tc:SAML:1.0:assertion""/>"
                };
            }
        }


        #endregion

        #region SamlEvdience
        public static SamlEvidenceTestSet SamlEvidenceMissAssertionIDRefAndAssertion
        {
            get
            {
                return new SamlEvidenceTestSet
                {
                    Xml = XmlGenerator.SamlEvidenceXml(null, null)
                };
            }
        }

        public static SamlEvidenceTestSet SamlEvidenceWithAssertionIDRef
        {
            get
            {
                return new SamlEvidenceTestSet
                {
                    Evidence = new SamlEvidence(new string[] { Default.SamlAssertionID }),
                    Xml = XmlGenerator.SamlEvidenceXml(XmlGenerator.SamlAssertionIDRefXml(Default.SamlAssertionID), null)
                };
            }
        }

        public static SamlEvidenceTestSet SamlEvidenceWithAssertions
        {
            get
            {
                return new SamlEvidenceTestSet
                {
                    Evidence = new SamlEvidence(new List<SamlAssertion> { SamlAssertionNoSignature.Assertion }),
                    Xml = XmlGenerator.SamlEvidenceXml(null, SamlAssertionNoSignature.Xml)
                };
            }
        }

        public static SamlEvidenceTestSet SamlEvidenceWithWrongElement
        {
            get
            {
                return new SamlEvidenceTestSet
                {
                    Xml = XmlGenerator.SamlEvidenceXml(SamlActionValid.Xml, null)
                };
            }
        }

        public static SamlEvidenceTestSet SamlEvidenceWithAssertionIDRefAndAssertions
        {
            get
            {
                return new SamlEvidenceTestSet
                {
                    Evidence = new SamlEvidence(new string[] { Default.SamlAssertionID }, new List<SamlAssertion> { SamlAssertionNoSignature.Assertion }),
                    Xml = XmlGenerator.SamlEvidenceXml(XmlGenerator.SamlAssertionIDRefXml(Default.SamlAssertionID), SamlAssertionNoSignature.Xml)
                };
            }
        }

        public static SamlEvidenceTestSet SamlEvidenceEmpty
        {
            get
            {
                return new SamlEvidenceTestSet
                {
                    Xml = "<Evidence xmlns=\"urn:oasis:names:tc:SAML:1.0:assertion\"/>"
                };
            }
        }
        #endregion

        #region SamlSubject
        public static SamlSubjectTestSet SamlSubjectNameIdentifierNull
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(null, null, null, new List<string> { Default.SamlConfirmationMethod }, Default.SamlConfirmationData),
                    Xml = XmlGenerator.SamlSubjectXml(null, XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) },
                                        Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectNoNameQualifier
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(Default.NameIdentifierFormat, null, Default.Subject, new List<string> { Default.SamlConfirmationMethod }, Default.SamlConfirmationData),
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(null, Default.NameIdentifierFormat, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectNoFormat
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(null, Default.NameQualifier, Default.Subject, new List<string> { Default.SamlConfirmationMethod }, Default.SamlConfirmationData),
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, null, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectNameNull
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, null),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectNameIDNotAbsoluteURI
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject("urn:oasis:names:tc:SAML:1.1:nameid-format:entity", null, "test", new List<string> { Default.SamlConfirmationMethod }, Default.SamlConfirmationData),
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(null, "urn:oasis:names:tc:SAML:1.1:nameid-format:entity", "test"),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectNameEmptyString
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, string.Empty),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectNoConfirmationData
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(Default.NameIdentifierFormat, Default.NameQualifier, Default.Subject, new List<string> { Default.SamlConfirmationMethod }, null),
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, null))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectConfirmationMethodNull
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(null, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectConfirmationMethodEmptyString
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(string.Empty) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectWithNameIdentifierAndConfirmation
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(Default.NameIdentifierFormat, Default.NameQualifier, Default.Subject, new List<string> { Default.SamlConfirmationMethod }, Default.SamlConfirmationData),
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectWithMultiConfirmationMethods
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(Default.NameIdentifierFormat, Default.NameQualifier, Default.Subject, new List<string> { Default.SamlConfirmationMethod, Default.SamlConfirmationMethod }, Default.SamlConfirmationData),
                    Xml = XmlGenerator.SamlSubjectXml(XmlGenerator.SamlNameIdentifierXml(Default.NameQualifier, Default.NameIdentifierFormat, Default.Subject),
                                XmlGenerator.SamlSubjectConfirmationXml(new List<string> { XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod), XmlGenerator.SamlConfirmationMethodXml(Default.SamlConfirmationMethod) }, Default.SamlConfirmationData))
                };
            }
        }

        public static SamlSubjectTestSet SamlSubjectEmpty
        {
            get
            {
                return new SamlSubjectTestSet
                {
                    Subject = new SamlSubject(),
                    Xml = "<Subject xmlns =\"urn:oasis:names:tc:SAML:1.0:assertion\"/>"
                };
            }
        }
        #endregion

        #region SamlSecurityToken
        public static SamlTokenTestSet SamlSecurityTokenValid
        {
            get => new SamlTokenTestSet
            {
                SecurityToken = SamlSecurityToken,
                Xml = ReferenceTokens.SamlToken_Formated
            };
        }

        public static SamlTokenTestSet TokenClaimsIdentitiesSubjectEmptyString
        {
            get => new SamlTokenTestSet
            {
                SecurityToken = new SamlSecurityToken(new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), null, null, new List<SamlStatement> { ReferenceSaml.GetAttributeStatement(new SamlSubject(), Default.Claims) }))
            };
        }

        public static SamlTokenTestSet TokenClaimsIdentitiesSameSubject
        {
            get
            {
                var claim = new Claim(ClaimTypes.Country, Default.Country, ClaimValueTypes.String, Default.Issuer);
                int index = ClaimTypes.Country.LastIndexOf('/');
                string ns = ClaimTypes.Country.Substring(0, index);
                string name = ClaimTypes.Country.Substring(index + 1);
                var statement = new SamlAttributeStatement(ReferenceSaml.SamlSubject, new SamlAttribute(ns, name, Default.Country));

                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                identity.AddClaim(claim);
                identity.AddClaim(claim);
                return new SamlTokenTestSet
                {
                    Identities = new List<ClaimsIdentity> { identity },
                    SecurityToken = new SamlSecurityToken(new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), null, null, new List<SamlStatement> { statement, statement })),
                };
            }
        }

        public static SamlTokenTestSet TokenClaimsIdentitiesDifferentSubjects
        {
            get
            {
                var claim1 = new Claim(ClaimTypes.Country, Default.Country, ClaimValueTypes.String, Default.Issuer);
                int index = ClaimTypes.Country.LastIndexOf('/');
                string ns = ClaimTypes.Country.Substring(0, index);
                string name = ClaimTypes.Country.Substring(index + 1);
                var attrStatement1 = new SamlAttribute(ns, name, Default.Country);
                var statement1 = new SamlAttributeStatement(ReferenceSaml.SamlSubject, attrStatement1);
                var identity1 = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                identity1.AddClaim(claim1);

                // statement2 has different subject with statement1
                var statement2 = new SamlAttributeStatement(new SamlSubject(Default.NameIdentifierFormat, Default.NameQualifier, Default.AttributeName), attrStatement1);
                var identity2 = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                identity2.AddClaim(claim1);

                var claim2 = new Claim(ClaimTypes.NameIdentifier, Default.AttributeName, ClaimValueTypes.String, Default.Issuer);
                claim2.Properties[ClaimProperties.SamlNameIdentifierFormat] = Default.NameIdentifierFormat;
                claim2.Properties[ClaimProperties.SamlNameIdentifierNameQualifier] = Default.NameQualifier;
                identity2.AddClaim(claim2);

                var claim3 = new Claim(ClaimTypes.AuthenticationMethod, Default.AuthenticationMethod, ClaimValueTypes.String, Default.Issuer);
                var claim4 = new Claim(ClaimTypes.AuthenticationInstant, Default.AuthenticationInstant, ClaimValueTypes.DateTime, Default.Issuer);

                // statement3 has same subject with statement1
                var statement3 = new SamlAuthenticationStatement(ReferenceSaml.SamlSubject, Default.AuthenticationMethod, DateTime.Parse(Default.AuthenticationInstant), null, null, null);
                identity1.AddClaim(claim3);
                identity1.AddClaim(claim4);

                return new SamlTokenTestSet
                {
                    Identities = new List<ClaimsIdentity> { identity1, identity2 },
                    SecurityToken = new SamlSecurityToken(new SamlAssertion(Default.SamlAssertionID, Default.Issuer, DateTime.Parse(Default.IssueInstantString), null, null, new List<SamlStatement> { statement1, statement2, statement3 }))
                };
            }
        }

        public static SamlTokenTestSet JwtToken
        {
            get => new SamlTokenTestSet
            {
                SecurityToken = new JwtSecurityToken(Default.AsymmetricJwt)
            };
        }

        public static SamlTokenTestSet NullToken
        {
            get => new SamlTokenTestSet
            {
                SecurityToken = null,
            };
        }

        public static SamlTokenTestSet NullXmlWriter
        {
            get => new SamlTokenTestSet
            {
                SecurityToken = SamlSecurityToken,

            };
        }
        #endregion

        // SAML
        #endregion

    }
}
