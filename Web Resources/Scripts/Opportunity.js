function OnLoad() {
    ShowAndHideTransferDetailsTab();
}
function OnOpportunityTypeChange() {
    ShowAndHideTransferDetailsTab();
}
function ShowAndHideTransferDetailsTab() {
    var opportunityTypeValue = Xrm.Page.getAttribute("hisc_opportunitytype").getValue()
    switch (opportunityTypeValue) {
        case 660730000:  //New
            Xrm.Page.ui.tabs.get("tab_TransferDetails").setVisible(false);
            break;
        case 660730001:  //Transfer
            Xrm.Page.ui.tabs.get("tab_TransferDetails").setVisible(true);
            break;
        default:  //New
            Xrm.Page.ui.tabs.get("tab_TransferDetails").setVisible(false);
            break;
    }
}