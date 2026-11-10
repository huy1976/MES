import clsx from 'clsx';
import styles from './SearchInput.module.scss';
import {AiOutlineSearch} from 'react-icons/ai';
import React, { useRef } from 'react';

const SearchInput = ({className = "", style, placeholder}) => {
    const inputRef = useRef(null);

    const handleClick = () => {
      inputRef.current.focus();
    }

    return (
      <div className={clsx(styles.searchInput, className)} style={style}
           onClick={handleClick} >
        <AiOutlineSearch className={styles.icon}/>
        <input type="text" placeholder={placeholder} className={styles.input} ref={inputRef} />
      </div>
    );
  };
  
  export default SearchInput;
  