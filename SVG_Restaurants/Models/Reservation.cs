                            @if (ViewBag.LoyaltyPoints <= 1500)
                            {
                                @if (ViewBag.LoyaltyPoints >= 500)
                                {
                                    if (ViewBag.Status == "Gold")
                                    {
                                        <span>
                                            Congratulations! You are a Gold Tier member!
                                        </span>

                                    }
                                    else
                                    {
                                        <span>
                                            Congratulations! You are a Silver Tier member!
                                        </span>
                                        <span>
                                            You have <b class="text-orange-500">@sum</b> more points to be Gold Tier.
                                        </span>
                                    }

                                }
                                else
                                {
                                     if (ViewBag.Status == "Gold")
                                    {
                                        <span>
                                            Congratulations! You are a Gold Tier member!
                                        </span>

                                    }
                                    <span>
                                        <b class="text-orange-500">@silverSum</b> more points to be Silver Tier.
                                    </span>
                                }
                            }
                            else
                            {

                                    <span>
                                        Congratulations! You are a Gold Tier member!
                                    </span>
                              

                            }