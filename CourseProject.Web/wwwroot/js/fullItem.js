const likedIcon = '<svg width="24" height="24" viewBox="0 0 21 20" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">\n' +
    '                    <defs></defs>\n' +
    '                    <g id="liked" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\n' +
    '                        <g id="Dribbble-Light-Preview" transform="translate(-259.000000, -760.000000)" fill="#000000">\n' +
    '                            <g id="icons" transform="translate(56.000000, 160.000000)">\n' +
    '                                <path d="M203,620 L207.200006,620 L207.200006,608 L203,608 L203,620 Z M223.924431,611.355 L222.100579,617.89 C221.799228,619.131 220.638976,620 219.302324,620 L209.300009,620 L209.300009,608.021 L211.104962,601.825 C211.274012,600.775 212.223214,600 213.339366,600 C214.587817,600 215.600019,600.964 215.600019,602.153 L215.600019,608 L221.126177,608 C222.97313,608 224.340232,609.641 223.924431,611.355 L223.924431,611.355 Z" id="like-[#1385]"></path>\n' +
    '                            </g>\n' +
    '                        </g>\n' +
    '                    </g>\n' +
    '                </svg>';
const notLikedIcon = '<svg id="not-liked" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><defs><style>.a{fill:none;stroke:#000;stroke-linecap:round;stroke-linejoin:round;}</style></defs><title>Like</title><path class="a" d="M20,15.659h0a1.5,1.5,0,1,1,0,3H19a1.5,1.5,0,0,1,1.5,1.5c0,.829-.672,1-1.5,1H12.5c-2.851,0-3.5-.5-7-1v-8.5c2.45,0,6.5-4.5,6.5-8.5,0-1.581,2.189-2.17,3,.719.5,1.781-1,5.281-1,5.281h8a1.5,1.5,0,0,1,1.5,1.5c0,.829-.672,2-1.5,2H21a1.5,1.5,0,0,1,0,3H20"/><rect class="a" x="0.5" y="10.159" width="5" height="12"/><path d="M3.25,19.159a.75.75,0,1,0,.75.75.75.75,0,0,0-.75-.75Z"/></svg>';
$('#like-button').click(function () {
    let itemId = $(this).attr('data-item-id');
    $.ajax({
        url: '/Items/Like',
        type: 'POST',
        data: {
            itemId: itemId
        },
        timeout: 10000,
        success: function (likesCount) {
            if( $('#liked').length>0){
                $('#like-icon').html(notLikedIcon);
                
            }else{
                $('#like-icon').html(likedIcon);
            }
            $('#likes-count').text(likesCount);
            
        }
    });
});